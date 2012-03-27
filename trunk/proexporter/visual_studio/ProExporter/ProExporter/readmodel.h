#ifndef READMODEL_H
#define READMODEL_H

// Pro/Toolkit Includes
extern "C"
{
#include "ProToolkit.h"
#include "ProMdl.h"
#include "ProFeature.h"
#include "ProSolid.h"
#include "ProFeatType.h"
}

// Standard C++ Includes
#include <vector>
#include <string>

namespace proexporter
{
ProError exportModel2XML(ProMdl model, const char* outDir);

ProError writeFeatureInfoAction(ProFeature* feature, ProError status, ProAppData appData);

/** Write XML of feature elem trees in model */
ProError exportFeatElemTrees(ProMdl model, const char* outDir);

/** Add feature Ids from model to featIds */
ProError addFeatIds(ProMdl model, std::vector<int>* featIds);

ProError addFeatIdAction(ProFeature* feature, ProError eStatus, ProAppData aData);

ProError createElemTreeAction(ProFeature* feature, ProError eStatus, ProAppData appData);

ProError writeElemTreeAction(ProFeature* feature, ProError eStatus, ProAppData appData);

void listFeatureIds(ProMdl model);

std::string filepathFeature(ProFeature* feature, const char* outDir);

std::string filepathSection(ProSection& section, const char* outDir, ProFeature* feature);

ProError writeSectionsAction(ProFeature* feature, ProError status, ProAppData appData);

void writeSection(ProSection& section, const char* outDir, ProFeature* parentFeature);
} // end namespace proexporter
#endif
