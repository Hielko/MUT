#!/bin/sh

#call me in etc/rc.local

export workdir="/home/pi/share" 

cd $workdir

while true; do
#--------------------------------------------   
   if [ -d $workdir"/reboot" ]; then
     yes | rmdir reboot
     sudo reboot
   fi
#--------------------------------------------      
   if [ -d $workdir"/stop" ]; then
     sudo rm -rf stop
	 sudo pkill -F myservice.lock
	 sudo rm myservice.lock
   fi
#--------------------------------------------   
   if [ -d $workdir"/start" ]; then
     sudo rm -rf start
     if [ ! -d $workdir"/myservice.lock" ]; then
       sudo ./run.sh
	 fi
   fi

   if [ -d $workdir"/update" ]; then
     
     # Found new version, wait 20 secs for filecopy ... 
     sleep 20
	 
	 # Save current workdir
	 if [ -d $workdir"/backups" ]; then
	   echo "create backups dir"
	 else  
	   mkdir backups
	 fi
	 tarfilename=$(date +"%Y-%m-%d-%H-%M")".tar.gz"
     sudo tar -czf $tarfilename ./*
	 mv $tarfilename backups
	 
	 #mkdir copy"$tarfilename"
	 #cp * copy"$tarfilename"
	 
	 # copy update files
     yes | cp -rf update/* ./
     sudo rm -rf update
	 sleep 5
	 sudo ./run.sh
  fi
  
  sleep 15
done & 
