### xyz-sort
This utility sorts text file in order to fulfill [GDAL ASCII Gridded XYZ](http://www.gdal.org/frmt_xyz.html) specification.
It places cells with same Y coordinates on consecutive lines.
For a same Y coordinate value it organizes the lines in the dataset by increasing X values.
The supported column separators are: space, comma, semicolon and tabulations.  
  
Usage:
```bat  
   xyz-sort <source.xyz> [sorted.xyz]  
```
  
  
This utility is part of [GDAL Drag 'n Drop](https://github.com/kubaszostak/gdal-dragndrop) package.