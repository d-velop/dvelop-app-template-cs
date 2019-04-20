#!/bin/bash
umask 0000
cp /build/Makefile /buildinternal/
/bin/bash -c "/usr/bin/make $*"
