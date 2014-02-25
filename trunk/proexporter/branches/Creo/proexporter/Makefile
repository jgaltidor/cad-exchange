# Followed style of NT Makefiles in Protoolkit directory
# File Name
# altidor
MAKEFILENAME = Makefile

# Machine Type
PRO_MACHINE_TYPE = i486_nt

# Executable names
# altidor
EXEBASENAME = proexporter
EXE = $(EXEBASENAME).exe
EXE_DLL = $(EXEBASENAME).dll

# Pro/Toolkit Source & Machine Loadpoint  (EXTERNAL USE - DEFAULT)
# altidor: Had to add quotes around paths w/ spaces for -I flag
PROTOOL_SRC = C:/Program Files/proeWildfire 4.0/protoolkit
PROTOOL_SYS = $(PROTOOL_SRC)/$(PRO_MACHINE_TYPE)

# Pro/Toolkit Source & Machine Loadpoint  (PTC INTERNAL USE)
#PROTOOL_SRC = $(PTCSRC)/protoolkit
#PROTOOL_SYS = $(PTCSYS)

# Include File Paths
# altidor: Had to add quotes around paths that could contain spaces
INCS = -I. -I"$(PROTOOL_SRC)/protk_appls/includes" -I"$(PROTOOL_SRC)/includes" 

# Compiler Flags
CC = cl
MACH = -DPRO_MACHINE=29 -DPRO_OS=4
# altidor:
#   Added /EHsc flag to CCFLAGS because Visual C++ needs this flag
#   to compile C++ code that can throw exceptions
#   Enabling use of debugger by adding /Zi flag to CCFLAGS
CCFLAGS = -c -fp:precise -DUSE_ANSI_IOSTREAMS -D_USE_32BIT_TIME_T /EHsc /Zi
CFLAGS = $(CCFLAGS) $(INCS) $(MACH)

# Libraries
PTCLIBS = "$(PROTOOL_SYS)/obj/protoolkit.lib"
PTCLIBS_DLL = "$(PROTOOL_SYS)/obj/protk_dll.lib"
LIBS = libcmt.lib kernel32.lib user32.lib wsock32.lib advapi32.lib mpr.lib winspool.lib netapi32.lib psapi.lib gdi32.lib shell32.lib comdlg32.lib ole32.lib

# Object files
OBJS = readmodel.obj proexportermain.obj section2dwriter.obj tabprinter.obj utils.obj


# Linker
LINK = link

$(EXE) :  $(OBJS) $(PTCLIBS)
	@echo Start at `date`
	@echo PROTOOL_SRC = $(PROTOOL_SRC)
        @echo PROTOOL_SYS = $(PROTOOL_SYS)
#Executable applications compiled using WinMain() instead of main() should set the subsystem to windows instead of console.
# altidor:
#   Enabling use of debugger by adding /debug flag to LINK commands
	$(LINK) /subsystem:console -out:$(EXE) /debug /machine:i386 @<<longline.list
$(OBJS) $(PTCLIBS) $(LIBS)
<<
	
	@echo Finished at `date`
	del $(OBJS)

# Dll target
dll: $(EXE_DLL)

$(EXE_DLL) :  $(OBJS) $(PTCLIBS_DLL)
	@echo Start at `date`
	@echo PROTOOL_SRC = $(PROTOOL_SRC)
        @echo PROTOOL_SYS = $(PROTOOL_SYS)
# altidor:
#   Enabling use of debugger by adding /debug flag to LINK commands
	$(LINK) /subsystem:console -out:$(EXE_DLL) /debug /machine:i386 /dll @<<longline.list
$(OBJS) $(PTCLIBS_DLL) $(LIBS)
<<

	@echo Finished at `date`
	del $(OBJS)

# object dependencies

proexportermain.obj: proexportermain.cpp
	$(CC) $(CFLAGS) proexportermain.cpp

readmodel.obj:  readmodel.cpp readmodel.h
	$(CC) $(CFLAGS) readmodel.cpp

section2dwriter.obj:  section2dwriter.cpp section2dwriter.h
	$(CC) $(CFLAGS) section2dwriter.cpp
	
tabprinter.obj:  tabprinter.cpp tabprinter.h
	$(CC) $(CFLAGS) tabprinter.cpp

utils.obj:  utils.cpp utils.h
	$(CC) $(CFLAGS) utils.cpp

clean:
	del $(OBJS) $(EXEBASENAME).lib $(EXEBASENAME).exp $(EXEBASENAME).ilk $(EXEBASENAME).pdb $(EXEBASENAME).suo vc90.pdb

distclean: clean
	del $(EXE)

distclean_dll: clean
	del $(EXE_DLL)

rebuild: distclean
	nmake -f $(MAKEFILENAME)

rebuild_dll: distclean_dll
	nmake -f $(MAKEFILENAME) dll
	

	