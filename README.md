# NATAutoMapper

This application allows to manage the ports on router that support Upnp.

With this program you can:

1) open ports
2) open the ports associated with a known service
3) close ports
4) list the currently open ports (only opened with Upnp)

Command line arguments and scripts are supported.

The supported commands are the following (must be first in the command line):

1) -add
2) -remove

For either of these commands the following parameters must be given (in any order):

- /protocol: TCP | UDP | Both
- /privateport: a value between 1 and 65535
- /publicport: same as preceding parameter
- /ipaddress: the local address of the device, "Local" for the current device
- /lifetime : any value between 0 (infinite) to 2147483647, expressed in seconds
- /description: rule description

Script files are text files encoded in UTF-8 or UTF-32 (both big-endian and little-endian are supported).
