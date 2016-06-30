VBoxManage createvm --name "meme_server" --ostype "Ubuntu_64" --register --basefolder /public/home/jiangw/vbox/server1
VBoxManage modifyvm "meme_server" --memory 307200 --vram 8 --acpi on --ioapic on --hwvirtex on --vtxvpid on --cpus 20 --cpuhotplug on --chipset ich9 --boot1 disk  --boot2 dvd --nic1 bridged --bridgeadapter1 eth0 --nictype1 virtio --mouse usb --keyboard usb --vrde off

VBoxManage storagectl "meme_server" --name "SATA Controller" --add sata --hostiocache on --bootable on
VBoxManage storageattach "meme_server" --storagectl "SATA Controller" --port 0 --device 0 --type hdd --medium /public/home/jiangw/vbox/server1/server.hdd

