#include "readmodel.h"
#include "utils.h"

// Pro/Toolkit Includes
extern "C"
{
#include "ProCore.h"
#include "ProTKRunTime.h"
#include "ProUtil.h"
}

// Standard C++ includes
#include <string>
#include <iostream>

// Platform dependent include
#include <windows.h> // for creating directory

using namespace std;
using namespace proexporter;

/** Assumes that args are preceded by '+' or '-',
  * as for synchronous mode; this is explained on
  * p. 85 tkuse.pdf (ProToolkit User's Guide)
  */
char* getProtkArg(char* argv[], const int index) {
	char* arg = argv[index];
	arg++; // to ignore the first character ('+' or '-')
	return arg;
}

void processPrtFile(string proe_exe, string partfilepath) {
	string partName = getPartName(partfilepath);
	if(partName == "") {
		cerr << "Failed determining part name for file: " 
		     << partfilepath << endl;
		return;
	}
	string outDir = "pxml_";
	outDir += withOutExt(partName);

	// Creating command to start Pro/E
	string cmd = "\"";
	cmd += proe_exe;
	cmd += "\" "; // creates Pro/E Window
	//cmd += "\" -g:no_graphics -i:rpc_input "; // does not create Pro/E Window
	cmd += partfilepath;
	cmd += " +"; cmd += partName;
	cmd += " +"; cmd += outDir;

	// Start Pro/E
	char* start_cmd = (char*) cmd.c_str();
	
	cout << "Processing part file: " << partfilepath << endl;
	// cout << "Part name: " << partName << endl;
	// cout << "Output directory for XML files: " << outDir << endl;
	cout << "Command launching ProE: " << start_cmd << endl;

	ProError err = ProEngineerStart(start_cmd, "");
	errchk("processPrtFile", "ProEngineerStart", err);
	
	cout << "Finished processing part file: " << partfilepath << endl;
	cout << endl;
}

/** Start Pro/ENGINEER to run user_interactive */
int main(int argc, char *argv[])
{
    if(argc < 3) {
		cerr << "Usage: " << argv[0]
		     << " <proe.exe_path> <part_file1> <part_file2> ... <part_fileN>" << endl;
		exit(1);
    }
	// Read command line arguments
	char* proe_exe = argv[1];

	cout << "Path to proe.exe: " << proe_exe << endl;

	for(int i = 2; i < argc; i++) {
		processPrtFile(proe_exe, argv[i]);
	}

	cout << "proexporter completed successfully" << endl;
	return 0;
}

// Implementing user_initialize and user_terminate.
// These functions need to be prefixed with "extern "C"" as described
// on p. 83 of tkuse.pdf

extern "C" int user_initialize(int argc, char* argv[])
{
    if(argc < 3) {
		ProTKFprintf(stderr, "Usage: %s +<part_name> +<output directory>\n", argv[0]);
		return 1;
    }
	// Read command line arguments
	char* partnameArg = getProtkArg(argv, 1);
	//char* outDir = getProtkArg(argv, 2);
	char* outDir = "pxml";

	ProTKPrintf("Part name: %s\n", partnameArg);
	ProTKPrintf("Output directory for XML files: %s\n", outDir);

	// Create output directory
	CreateDirectory(outDir, NULL);

	// convert a file path string to representation used to Pro/E
	ProName partname;
	ProStringToWstring(partname, partnameArg);

	// Retrieve an part with that name
	ProMdl part;
    ProError err = ProMdlRetrieve(partname, PRO_MDL_PART, &part);
    if (err != PRO_TK_NO_ERROR) {
		ProTKFprintf(stderr, "*** Failed to retrieve part %s\n", partnameArg);
        ProEngineerEnd();
		return 1;
    }
	// list all of the feature ids
	exportModel2XML(part, outDir);
	// Terminate the Pro/ENGINEER session.
	ProEngineerEnd();
	return 0;
}

extern "C" void user_terminate() {
	cout << "Pro/TOOLKIT application terminated successfully" << endl;
}

