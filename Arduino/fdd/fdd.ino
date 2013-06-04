
int delay_time = 0, pos = 0, dir = 1, toggle = 0;

void setup()
{
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(13, OUTPUT);

  reset();
  
  Serial.begin(9600);
}

void serialEvent()
{
  digitalWrite(13, toggle ? LOW : HIGH);
  toggle = ~toggle;
  while (Serial.available() < 2);
  int high = Serial.read();
  int low = Serial.read();
  delay_time = (high << 8) | low;
  Serial.write(42);
}

void reset()
{
  digitalWrite(2, HIGH);
  for (int i = 0; i < 100; i++)
  {
    digitalWrite(3, LOW);
    digitalWrite(3, HIGH);
    delay(5);
  }  
  
  digitalWrite(2, LOW);
  for (int i = 0; i < 30; i++)
  {
    digitalWrite(3, LOW);
    digitalWrite(3, HIGH);
    delay(5);
  } 
  
  delay(1000);
}

void delay_us(int us)
{
  while  (us > 10000)
  {
    delayMicroseconds(10000);
    us -= 10000;
  }
  
  if (us > 0)
  {
    delayMicroseconds(us);
  }
}

void loop()
{
  if (delay_time > 0)
  {
    pos += dir;
    
    if (pos < 0 || pos >= 80)
    {
      dir *= -1;
      pos += dir + dir;
    }
    
    dir *= -1;
    
    digitalWrite(2, dir == 1 ? LOW : HIGH);
    digitalWrite(3, LOW);
    digitalWrite(3, HIGH);
    
    delay_us(delay_time);
  }
}

