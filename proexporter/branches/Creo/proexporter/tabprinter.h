#ifndef TABPRINTER_H
#define TABPRINTER_H

#include <cstdio>
#include <string>

namespace proexporter
{
class TabPrinter
{
public:
	TabPrinter(FILE* fp, const std::string& tab);

	void printf(const char* fmtstr, ...);

	void printlnf(const char* fmtstr, ...);
	
	void println();
	
	void incrementTabs();
	
	void decrementTabs();
	
	void close();

	FILE* getFileDescriptor();
	
	bool isStartingNewLine();
	
	void printTabs();

private:

	std::string tab;
	FILE* fp;
	int numTabs;
	bool startingNewLine;
};
} // end namespace proexporter
#endif
