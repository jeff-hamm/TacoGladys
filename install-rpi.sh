#wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
#sudo dpkg --purge packages-microsoft-prod && sudo dpkg -i packages-microsoft-prod.deb
#sudo apt-get update
#sudo add-apt-repository universe
#sudo apt-get update
#sudo apt-get install apt-transport-https
#sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
sudo apt-get install aspnetcore-runtime-3.1
sudo apt-get install dotnet-runtime-3.1


#wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
#sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
#wget -q https://packages.microsoft.com/config/debian/9/prod.list
#sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
#sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
#sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list

wget https://download.visualstudio.microsoft.com/download/pr/67766a96-eb8c-4cd2-bca4-ea63d2cc115c/7bf13840aa2ed88793b7315d5e0d74e6/dotnet-sdk-3.1.100-linux-arm.tar.gz
wget https://download.visualstudio.microsoft.com/download/pr/8c839c0e-a5ae-4254-8d8b-c012528fe601/c147e26bad68f97eacc287a71e01331d/aspnetcore-runtime-3.1.0-linux-arm.tar.gz


mkdir dotnet-arm32
tar zxf aspnetcore-runtime-3.1.0-linux-arm.tar.gz -C $HOME/dotnet-arm32
tar zxf dotnet-sdk-3.1.100-linux-arm.tar.gz -C $HOME/dotnet-arm32