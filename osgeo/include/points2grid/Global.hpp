/*
*
COPYRIGHT AND LICENSE

Copyright (c) 2011 The Regents of the University of California.
All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

1. Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following
disclaimer in the documentation and/or other materials provided
with the distribution.

3. All advertising materials mentioning features or use of this
software must display the following acknowledgement: This product
includes software developed by the San Diego Supercomputer Center.

4. Neither the names of the Centers nor the names of the contributors
may be used to endorse or promote products derived from this
software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS''
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS
OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
*
*/

#pragma once

static const unsigned int OUTPUT_TYPE_MIN = 0x00000001;
static const unsigned int OUTPUT_TYPE_MAX = 0x00000010;
static const unsigned int OUTPUT_TYPE_MEAN = 0x00000100;
static const unsigned int OUTPUT_TYPE_IDW = 0x00001000;
static const unsigned int OUTPUT_TYPE_DEN = 0x00010000;
static const unsigned int OUTPUT_TYPE_STD = 0x00100000;
static const unsigned int OUTPUT_TYPE_ALL = 0x00111111;

enum OUTPUT_FORMAT {
    OUTPUT_FORMAT_ALL = 0,
    OUTPUT_FORMAT_ARC_ASCII,
    OUTPUT_FORMAT_GRID_ASCII,
    OUTPUT_FORMAT_GDAL_GTIFF
};

enum INPUT_FORMAT {
    INPUT_ASCII = 0,
    INPUT_LAS = 1
};

enum INTERPOLATION_TYPE {
    INTERP_AUTO = 0,
    INTERP_INCORE = 1,
    INTERP_OUTCORE = 2
};

