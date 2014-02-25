#include "TabPrinter.h"
#include <cstdarg>
#include <exception>
#include <cassert>

using namespace std;

proexporter::TabPrinter::TabPrinter(FILE* fp, const string& tab) :
	fp(fp), tab(tab), numTabs(0), startingNewLine(true)
{
}

void proexporter::TabPrinter::printf(const char* fmtstr, ...) {
	if(startingNewLine) printTabs();
	va_list args;
	va_start(args, fmtstr);
	vfprintf(fp, fmtstr, args);
	va_end(args);
}

void proexporter::TabPrinter::printlnf(const char* fmtstr, ...) {
	if(startingNewLine) printTabs();
	va_list args;
	va_start(args, fmtstr);
	vfprintf(fp, fmtstr, args);
	this->println();
	va_end(args);
}

void proexporter::TabPrinter::println() {
	fputc('\n', fp);
	fflush(fp);
	startingNewLine = true;
}

void proexporter::TabPrinter::printTabs() {
	const char* tab_cstr = tab.c_str();
	for(int i = 0; i < numTabs; i++) { fprintf(fp, tab_cstr); }
	startingNewLine = false;
}

bool proexporter::TabPrinter::isStartingNewLine() { return startingNewLine; }

void proexporter::TabPrinter::incrementTabs() { numTabs++; }
	
void proexporter::TabPrinter::decrementTabs() { if(numTabs > 0) numTabs --; }

FILE* proexporter::TabPrinter::getFileDescriptor() { return fp; }

void proexporter::TabPrinter::close() {
	fflush(fp);
	fclose(fp);
}
