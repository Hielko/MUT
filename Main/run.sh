#!/bin/sh

cd /home/pi/share

if [ -f "myservice.lock" ]; then
  sudo pkill -F myservice.lock
  sudo rm myservice.lock
fi  

mono-service -l:myservice.lock ./IcqBot.exe &
