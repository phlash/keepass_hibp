# keepass_hibp
Alternative Have I Been Pwned? Plugin for KeePass2 that uses curl or wget to fetch hashes, avoiding mono's TLS limitations,
if you have to use mono before 4.8.0 (eg: Debian 9)

## Credits
Original work by Andrew Schofield: https://github.com/andrew-schofield/keepass2-haveibeenpwned

## Building
You will need mono-devel, and an installation of keepass2 (this module references the KeePass.exe binary). There is a trivial
Makefile, targets 'all' and 'clean' are probably useful :)

## Installing
As root, copy the Plugins/HIBP.dll to /usr/lib/keepass2/Plugins

## Using
With a database loaded, select Tools->Have I Been Pwned? This will iterate through all your credentials, checking k-anonymised
partial password hashes against the HIBP APIv3. Any matches will be flagged by a pop-up message box.

This version (1.1.1.0) includes [padding in the HIBP response](https://haveibeenpwned.com/API/v3#PwnedPasswordsPadding)
