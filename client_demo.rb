require 'socket'

s = TCPSocket.new 'localhost', 12345

def try_to_read s
	begin
		m = s.read_nonblock(4096)
	rescue EOFError
		exit 0;
	rescue Errno::EAGAIN, IO::WaitReadable, IO::EWOULDBLOCKWaitReadable
		IO.select([s])
	retry
end

return m
end

t = Thread.new {
	m = ""
	while (m != "BYE") do
		m  = try_to_read(s)
		m.strip!
		puts "[#{m.bytes.size} bytes]>>> '#{m}'"
	end
}


s.write "CONNECT Asuncion"

cmd = ""

while cmd != "EOF"
	cmd = gets
	cmd.strip!
	s.write_nonblock cmd
end

t.join

s.write "DISCONNECT"
