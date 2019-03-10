vboxmanage startvm meme_server -type headless
echo "please wait while virtualbox is starting your vm..."

perl -e 'sleep(30)'

echo you can using the ip address show below to connect to your started vm:
ip=$(vboxmanage guestproperty enumerate meme_server | grep V4/IP)
echo $ip
