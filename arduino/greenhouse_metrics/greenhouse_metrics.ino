#include <Wire.h>
#include <SoftwareSerial.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <I2CSoilMoistureSensor.h>
#include <dht.h>
#define dht_apin A0 // Analog Pin sensor is connected to
#define lightsensor_pin A1 // Analog pin sensor for light
#define soilsensor_pin A2 // Analog pin sensor for soilsensor
#define ONE_WIRE_BUS 2


OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);
SoftwareSerial BT(10, 9); // Arduino RX, TX
dht DHT;
int ledpin=13;
int lightSensorValue;
double soilSensorValue;
double lumenRatio=0.7;
double lumen;
const int lightcontroller= 3;

void setup(){
 
  Wire.begin();
  Serial.begin(9600); 
  delay(500);//Delay to let system boot
  sensors.begin();
  pinMode(lightcontroller,OUTPUT);
  delay(1000);//Wait before accessing Sensor
 
}
 
void loop(){

        
    soilSensorValue =  AverageValueOverIterations(soilsensor_pin,100); 
    Serial.print(soilSensorValue);
    if(soilSensorValue > 225 && soilSensorValue<600)
    {
      soilSensorValue = map(soilSensorValue,225,600,0.00,100.00); 
    }
    else{
      if(soilSensorValue<225){
        soilSensorValue =0.00;
      }
      if(soilSensorValue>600){
        soilSensorValue=100.00;
      }
    }
    DHT.read11(dht_apin);
    lightSensorValue = AverageValueOverIterations(lightsensor_pin,100); 
    lumen= lightSensorValue * lumenRatio;
    sensors.requestTemperatures();
    
    Serial.print(";");
    
    WriteValue("Humidity-Air",DHT.humidity,"%");
    WriteValue("Temperature-Air",DHT.temperature,"C");
    WriteValue("Temperature-Earth",sensors.getTempCByIndex(0),"C");
    WriteValue("Light",lumen,"Lumen");
    WriteValue("Humidity-Earth",soilSensorValue,"unknown");   
    Serial.println(";");  
    if(lumen <100.0){
      digitalWrite(lightcontroller,HIGH);
      delay(30);
      digitalWrite(lightcontroller,LOW);
    }
    delay(5000);//Wait 5 seconds before accessing sensor again. 
}

double AverageValueOverIterations(int pin, double iterations){
  double value = 0.0;
  for (int i = 0; i <= iterations; i++) 
  { 
    value = value + analogRead(pin); 
    delay(1); 
  } 
  
  value = value/iterations; 
  return value;
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
