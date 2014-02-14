#ifndef SECTION2DWRITER_H
#define SECTION2DWRITER_H

#include "tabprinter.h"

// Pro/Toolkit Includes
extern "C"
{
#include "ProSection.h"
#include "ProSecdimType.h"
#include "ProArray.h"
#include "ProTKRunTime.h"
}

namespace proexporter
{
class Section2dWriter
{
public:
	Section2dWriter(ProSection& section, TabPrinter& out);
	void exportXML();
	ProError writeEntities();
	ProError writeEntity(int entityId);
	ProError writeConstraints();
	ProError writeConstraint(int constraintId);
	ProError writeDimensions();
	ProError writeDimension(int dimensionId);
	
	void writeLocation(Pro2dPnt& location);
	void writeEntityReferences(const int* ref_ids, const int n_ref_ids);
	void writeEntityReference(const int ref_id);
	void writePointTypes(const ProSectionPointType* ref_ids, const int n_ref_ids);
	void writePointType(const ProSectionPointType& p_type);

	// Functions for writing entity structures

	void writePro2dPointdef(Pro2dPointdef* p_pnt);
	void writePro2dCoordSysdef(Pro2dCoordSysdef* p_coord);
	void writePro2dLinedef(Pro2dLinedef* p_line);
	void writePro2dClinedef(Pro2dClinedef* p_cline);
	void writePro2dArcdef(Pro2dArcdef* p_arc);
	void writePro2dCircledef(Pro2dCircledef* p_circle);
	void writePro2dPolylinedef(Pro2dPolylinedef* p_polyline);
	void writePro2dSplinedef(Pro2dSplinedef* p_spline);
	void writePro2dTextdef(Pro2dTextdef* p_text);
	void writePro2dBlendVertexdef(Pro2dBlendVertexdef* p_bvertex);
	void writePro2dEllipsedef(Pro2dEllipsedef* p_ellipse);
	void writePro2dConicdef(Pro2dConicdef* p_conic);

	// Functions for writing primitive Pro/E data

	void writePro2dPnt(Pro2dPnt& pnt);
	void writeProAngle(ProAngle& angle);
	void writeNPoints(unsigned int n_points);
	void writeComment(ProComment& comment);

private:
	ProSection& section;
	TabPrinter& out;
};
} // end namespace proexporter
#endif
