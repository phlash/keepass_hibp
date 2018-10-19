all: KeePass.exe Plugins/HIBP.dll

install: all
	install -o root -g root -m 755 -d /usr/lib/keepass2/Plugins
	install -o root -g root -m 755 Plugins/HIBP.dll /usr/lib/keepass2/Plugins

Plugins/HIBP.dll: HIBP.cs
	mkdir -p Plugins
	mcs -target:library -out:$@ -r:KeePass.exe -r:System.Windows.Forms -r:System.Drawing $<

KeePass.exe: /usr/lib/keepass2/KeePass.exe
	cp -p $< $@

test: all
	cli --trace=E:System.Exception ./KeePass.exe

clean:
	rm -f *~ .*~ Plugins/HIBP.dll KeePass.exe*
