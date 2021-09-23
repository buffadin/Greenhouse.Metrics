#include <dht.h>
#define dht_apin A0 // Analog Pin sensor is connected to
#define lightsensor_pin A1 // Analog pin sensor for light
 
dht DHT;
int val;
int ledpin=13;
int lightSensorValue;

void setup(){
 
  Serial.begin(9600);
  delay(500);//Delay to let system boot
  pinMode(ledpin,OUTPUT);  
  delay(1000);//Wait before accessing Sensor
 
}
 
void loop(){

   
    DHT.read11(dht_apin);
    lightSensorValue = analogRead(lightsensor_pin);
    
    Serial.print(";");
    WriteValue("Humidity",DHT.humidity,"%");
    WriteValue("Temperature-Air",DHT.temperature,"C");
    WriteValue("Light",lightSensorValue,"Unknown");
    Serial.println(";");
    delay(5000);//Wait 5 seconds before accessing sensor again. 
}

void WriteValue(String name,double value, String unit){
    Serial.print(name);
    Serial.print(" ");
    Serial.print(value);
    Serial.print(" ");
    Serial.print(unit);
    Serial.print(";");
    return true;
}
