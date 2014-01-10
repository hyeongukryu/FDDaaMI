
void init_pin(int pin)
{
  pinMode(pin, OUTPUT);
  digitalWrite(pin, LOW);
}

void init_pin_all()
{
  for (int i = 2; i <= 9; i++)
  {
    init_pin(i);
  }
  init_pin(13);
}

#define DELAY_US(x) \
do { \
  int us = x; \
  while (us > 10000) { \
    delayMicroseconds(10000); \
    us -= 10000; \
  } \
  if (us > 0) \
    delayMicroseconds(us); \
} while (0)


#define RESET_1_STEP 85
#define RESET_2_STEP 30
#define RESET_DELAY 4333

#define DIR_NORM  1
#define DIR_REV -1

char dir[4] = {DIR_NORM, DIR_NORM, DIR_NORM, DIR_NORM};

char fdd_dir_pin[4] = {2, 4, 6, 8};
char fdd_clk_pin[4] = {3, 5, 7, 9};

#define DIRECTION(pin_dir, dir) \
do { digitalWrite(pin_dir, dir == 1 ? LOW : HIGH); } while(0)

#define TOGGLE_FDD_DIRECTION(fdd) \
do { dir[fdd] = -dir[fdd]; } while(0)

#define FDD_DIRECTION(fdd) \
do { DIRECTION(fdd_dir_pin[fdd], dir[fdd]); } while (0)

#define MOVE_STEP(pin_clk) \
do { \
  digitalWrite(pin_clk, LOW); \
  digitalWrite(pin_clk, HIGH); \
} while(0)

#define MOVE_FDD_STEP(fdd) \
do { MOVE_STEP(fdd_clk_pin[fdd]); } while(0)

void reset(int pin_dir, int pin_clk)
{
  DIRECTION(pin_dir, DIR_REV);
  for (int i = 0; i < RESET_1_STEP; i++)
  {
    MOVE_STEP(pin_clk);
    DELAY_US(RESET_DELAY);
  }
  DIRECTION(pin_dir, DIR_NORM);
  for (int i = 0; i < RESET_2_STEP; i++)
  {
    MOVE_STEP(pin_clk);
    DELAY_US(RESET_DELAY);
  }
  DIRECTION(pin_dir, DIR_NORM);
}

void reset_all()
{
  for (int i = 0; i < 4; i++)
    reset(fdd_dir_pin[i], fdd_clk_pin[i]);
    
  delay(300);
}

int remaining_us[4] = {0, 0, 0, 0};
int designated_delay[4] = {0, 0, 0, 0};
int pos[4] = {30, 30, 30, 30};

#define SERIAL_OK 42

void setup()
{
  init_pin_all();
  delay(30);
  reset_all();

  Serial.begin(38400);
  while (Serial.available() > 0 && Serial.read() != SERIAL_OK);
}

bool led_on = false;
#define LED_TOGGLE() \
do { \
  led_on = !led_on; \
  digitalWrite(13, led_on ? HIGH : LOW); \
} while(0)

#define DESERIALIZE(high, low, result) \
do { result = (high << 8) | low; } while(0)

void serialEvent()
{  
  while (Serial.available() < 1);
  int channel = Serial.read();
  if ((0 <= channel && channel < 4) == false)
    return;

  while (Serial.available() < 2);
  int high = Serial.read();
  int low = Serial.read();

  LED_TOGGLE();
  
  DESERIALIZE(high, low, designated_delay[channel]);
  remaining_us[channel] = 0;
    
  Serial.write(SERIAL_OK);
}

int last_delay = 0;

#define MAX 0x7FFF

#define FDD_POS_MAX 80

#define MODE_INPLACE

void loop()
{
  int next_delay = MAX;
  
  for (char i = 0; i < 4; i++)
  {
    remaining_us[i] -= last_delay;
  
    if (remaining_us[i] <= 0)
    {
      if (designated_delay[i] != 0)
      {
#ifdef MODE_INPLACE
        TOGGLE_FDD_DIRECTION(i);
#else
        pos[i] += dir[i];
        if(pos[i] <= 0 || pos[i] >= FDD_POS_MAX)
        {
          pos[i] -= dir[i];
          TOGGLE_FDD_DIRECTION(i);
          pos[i] += dir[i]; 
        }
#endif
        FDD_DIRECTION(i);
        MOVE_FDD_STEP(i);
      }
      remaining_us[i] = designated_delay[i];
    }
    
    if (remaining_us[i] != 0 && next_delay > remaining_us[i])
      next_delay = remaining_us[i];
  }
  
  if (next_delay == MAX)
    next_delay = 0;
  
  last_delay = next_delay;
  DELAY_US(last_delay);
}
