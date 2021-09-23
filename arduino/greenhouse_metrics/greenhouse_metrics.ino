#include <dht.h>
#define dht_apin A0 // Analog Pin sensor is connected to
 
dht DHT;
int val;
int ledpin=13; 

void setup(){
 
  Serial.begin(9600);
  delay(500);//Delay to let system boot
  pinMode(ledpin,OUTPUT);  
  delay(1000);//Wait before accessing Sensor
 
}
 
void loop(){

   
    DHT.read11(dht_apin);
    Serial.print(";");
    Serial.print("Humidity ");
    Serial.print(DHT.humidity);
    Serial.print(" %");
    Serial.print(";");
    Serial.print("Temperature-Air ");
    Serial.print(DHT.temperature); 
    Serial.print(" C");
    Serial.println(";");
    
    delay(5000);//Wait 5 seconds before accessing sensor again.
 
 
}
