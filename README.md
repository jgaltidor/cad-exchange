CAD Data Exchange
===================
This package contains software tools for converting CAD part
file data.
For instance, `pro2sw` is a software tool that converts simple
2D sections
from [Pro/Engineer][proe]'s native file format to
[SolidWorks][sw]' format.
These tools demonstrate the usefulness and implement
the formal language approach to CAD interoperability presented
in an ASME IDETC
[publication](http://people.cs.umass.edu/~jaltidor/idetc2011.pdf).
It is important to note that this software tool is intended for
use converting simple 2D sections and not complete 3D part files.
These 2D sections can only contain straight lines or boxes created
using the line or rectangle tool. These lines do not need to
be horizontal or vertical. Line and line to point dimensions
can also be used. Simple constraints like "same point", "point
on entity", "horizontal" and "vertical" are covered by the conversion
rules.

[proe]: http://www.ptc.com/cad/3d-cad/creo-parametric
[sw]: http://www.solidworks.com/
