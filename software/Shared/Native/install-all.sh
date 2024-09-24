#!/bin/bash

echo "installing pcf-8523 overlay"
sudo ./sun50i-h616-pcf-8523-install.sh

echo "installing services"
sudo cp ./cog1_logo.service /etc/systemd/system
sudo cp ./cog1.service /etc/systemd/system
sudo chmod -x /etc/systemd/system/cog1_logo.service
sudo chmod -x /etc/systemd/system/cog1.service
sudo systemctl enable cog1_logo
sudo systemctl enable cog1
sudo mkdir /cog1