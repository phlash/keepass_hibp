# keepass_hibp
Alternative Have I Been Pwned? Plugin for KeePass2 that uses wget to fetch hashes, avoiding mono's TLS limitations

## Credits
Original work by Andrew Schofield: https://github.com/andrew-schofield/keepass2-haveibeenpwned

## Building
You will need mono-devel, and an installation of keepass2 (this module references the KeePass.exe binary). There is a trivial
Makefile, targets 'all' and 'clean' are probably useful :)

## Installing
As root, copy the Plugins/HIBP.dll to /usr/lib/keepass2/Plugins

## Using
With a database loaded, select Tools->Have I Been Pwned? This will iterate through all your credentials, checking k-anonymised
partial password hashes against the HIBP APIv2. Any matches will be flagged by a pop-up message box.
