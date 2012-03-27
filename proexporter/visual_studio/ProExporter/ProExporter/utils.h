#ifndef UTILS_H
#define UTILS_H

// Pro/Toolkit Includes
extern "C"
{
#include "ProToolkit.h"
#include "Pro2dEntdef.h"
#include "ProSection.h"
}

#include <string>

namespace proexporter
{
void errchk(const char* func, const char* call, const ProError err);

std::string getSectionName(const ProSection& section);
std::string getBoolStr(const ProBoolean& probool);
std::string getTypeStr(const Pro2dEntType& type);
std::string getTypeStr(const ProConstraintType& type);
std::string getStatusStr(const ProConstraintStatus& status);
std::string getTypeStr(const ProSecdimType& status);
std::string getTypeStr(const ProSectionPointType& type);
std::string filebasename(std::string filepath);
std::string withOutExt(std::string filepath);
std::string toUpperCase(std::string s);
std::string getPartName(std::string partfilepath);
} // end namespace proexporter
#endif
