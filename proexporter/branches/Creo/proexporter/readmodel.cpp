#include "readmodel.h"
#include "utils.h"
#include "tabprinter.h"
#include "section2dwriter.h"

// ProToolkit Includes
extern "C" {
#include "ProElement.h"
#include "ProUtil.h"
}

// Standard C++ Includes
#include <iostream>
#include <sstream>

using namespace std;

ProError proexporter::exportModel2XML(ProMdl model, const char* outDir) {
	ProError status = ProSolidFeatVisit((ProSolid) model,
	  (ProFeatureVisitAction) writeFeatureInfoAction, NULL, (ProAppData) outDir);
	errchk("addFeatIds", "ProSolidFeatureVisit", status);
	return status;
}

ProError proexporter::writeFeatureInfoAction(ProFeature* feature, ProError status, ProAppData appData) {
	writeElemTreeAction(feature, status, appData);
	writeSectionsAction(feature, status, appData);
	return status;
}


ProError proexporter::exportFeatElemTrees(ProMdl model, const char* outDir) {
	ProError status = ProSolidFeatVisit((ProSolid) model,
	  (ProFeatureVisitAction) writeElemTreeAction, NULL, (ProAppData) outDir);
	errchk("addFeatIds", "ProSolidFeatureVisit", status);
	return status;
}

ProError proexporter::addFeatIds(ProMdl model, vector<int>* featIds) {
	ProError status = ProSolidFeatVisit( (ProSolid) model,
	  (ProFeatureVisitAction) addFeatIdAction, NULL, (ProAppData) featIds);
	errchk("addFeatIds", "ProSolidFeatureVisit", status);
	return status;
}


ProError proexporter::addFeatIdAction(ProFeature* feature, ProError status, ProAppData appData)
{
	vector<int>* featIds = (vector<int>*) appData;
	featIds->push_back(feature->id);
	return PRO_TK_NO_ERROR;
}

ProError proexporter::writeElemTreeAction(ProFeature* feature, ProError status, ProAppData appData) {
	const char* outDir = (const char*) appData;
	ProElement elemtree;
	status = ProFeatureElemtreeCreate(feature, &elemtree);
	errchk("writeElemTreeAction", "ProFeatureElemtreeCreate", status);
	if(status != PRO_TK_NO_ERROR) {
		return PRO_TK_NO_ERROR;
	}
	char* xmlFilePath = (char*) filepathFeature(feature, outDir).c_str();
	remove(xmlFilePath); // delete file if it exists
	ProPath xmlFilePath_pro;
	ProStringToWstring(xmlFilePath_pro, xmlFilePath);
	
	// Write XML representation of feature
	cout << "Writing elem tree for feature " << feature->id << endl;
	status = ProElemtreeWrite(elemtree, PRO_ELEMTREE_XML, xmlFilePath_pro);
	errchk("writeElemTreeAction", "ProElemtreeWrite", status);
	if(status != PRO_TK_NO_ERROR) {
		return PRO_TK_NO_ERROR;
	}
	// Free elemtree
	status = ProElementFree(&elemtree);
	errchk("writeElemTreeAction", "ProElementFree", status);
	return PRO_TK_NO_ERROR;
}

void proexporter::listFeatureIds(ProMdl model)
{
	vector<int> featIds;
	addFeatIds(model, &featIds);
	vector<int>::iterator endIter = featIds.end();
	for(vector<int>::iterator iter = featIds.begin(); iter != endIter; iter++) {
		int id = *iter;
		printf("feature id: %d\n", id);
	}
}

string proexporter::filepathFeature(ProFeature* feature, const char* outDir) {
	ostringstream formatter;
	formatter << outDir << "\\feature" << feature->id << ".xml";
	return formatter.str();
}

string proexporter::filepathSection(ProSection& section, const char* outDir, ProFeature* parentFeature) {
	ostringstream formatter;
	formatter << outDir << "\\section_" << getSectionName(section) << "_feature";
	if(parentFeature != NULL) {
		formatter << parentFeature->id;
	}
	else {
		formatter << 'X';
	}
	formatter << ".xml";
	return formatter.str();
}

ProError proexporter::writeSectionsAction(ProFeature* feature, ProError status, ProAppData appData) {
	const char* outDir = (const char*) appData;
	int n_sections;
    status = ProFeatureNumSectionsGet(feature, &n_sections);
    errchk("ProFeatureNumSectionsGet", "writeSectionsAction", status);

	ProSection section;
	for(int i = 0; i < n_sections; i++) {
		status = ProFeatureSectionCopy(feature, i, &section);
		errchk("ProFeatureSectionCopy", "writeSectionsAction", status);
		writeSection(section, outDir, feature);
		status = ProSectionFree(&section);
	}
	return PRO_TK_NO_ERROR;
}

void proexporter::writeSection(ProSection& section, const char* outDir, ProFeature* parentFeature) {
	string filename = filepathSection(section, outDir, parentFeature);
	FILE* fp = fopen(filename.c_str(), "w");
	const char* tab = "  ";
	TabPrinter tp(fp, tab);
	cout << "Writing section XML file:  " << filename << endl;
	Section2dWriter writer(section, tp);
	writer.exportXML();
	fclose(fp);
	cout << "Finished writing " << filename << endl;
}

