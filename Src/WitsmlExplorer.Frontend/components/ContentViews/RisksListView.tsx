import React, { useContext, useEffect, useState } from "react";
import NavigationContext from "../../contexts/navigationContext";
import OperationContext from "../../contexts/operationContext";
import OperationType from "../../contexts/operationType";
import RiskObject from "../../models/riskObject";
import { getContextMenuPosition } from "../ContextMenus/ContextMenu";
import RiskObjectContextMenu, { RiskObjectContextMenuProps } from "../ContextMenus/RiskContextMenu";
import formatDateString from "../DateFormatter";
import { ContentTable, ContentTableColumn, ContentTableRow, ContentType } from "./table";
import { clipLongString } from "./ViewUtils";

export interface RiskObjectRow extends ContentTableRow, RiskObject {
  risk: RiskObject;
}

export const RisksListView = (): React.ReactElement => {
  const { navigationState } = useContext(NavigationContext);
  const {
    operationState: { timeZone }
  } = useContext(OperationContext);
  const { selectedWellbore, selectedServer, servers } = navigationState;
  const { dispatchOperation } = useContext(OperationContext);
  const [risks, setRisks] = useState<RiskObject[]>([]);

  useEffect(() => {
    if (selectedWellbore && selectedWellbore.risks) {
      setRisks(selectedWellbore.risks);
    }
  }, [selectedWellbore]);

  const getTableData = () => {
    return risks.map((risk) => {
      return {
        ...risk,
        ...risk.commonData,
        id: risk.uid,
        mdBitStart: `${risk.mdBitStart?.value?.toFixed(4) ?? ""} ${risk.mdBitStart?.uom ?? ""}`,
        mdBitEnd: `${risk.mdBitEnd?.value?.toFixed(4) ?? ""} ${risk.mdBitEnd?.uom ?? ""}`,
        dTimStart: formatDateString(risk.dTimStart, timeZone),
        dTimEnd: formatDateString(risk.dTimEnd, timeZone),
        details: clipLongString(risk.details, 30),
        summary: clipLongString(risk.summary, 40),
        risk: risk
      };
    });
  };

  const columns: ContentTableColumn[] = [
    { property: "type", label: "type", type: ContentType.String },
    { property: "sourceName", label: "commonData.sourceName", type: ContentType.String },
    { property: "mdBitStart", label: "mdBitStart", type: ContentType.String },
    { property: "mdBitEnd", label: "mdBitEnd", type: ContentType.String },
    { property: "dTimStart", label: "dTimStart", type: ContentType.DateTime },
    { property: "dTimEnd", label: "dTimEnd", type: ContentType.DateTime },
    { property: "name", label: "name", type: ContentType.String },
    { property: "summary", label: "summary", type: ContentType.String },
    { property: "severityLevel", label: "severityLevel", type: ContentType.String },
    { property: "category", label: "category", type: ContentType.String },
    { property: "subCategory", label: "subCategory", type: ContentType.String },
    { property: "affectedPersonnel", label: "affectedPersonnel", type: ContentType.String },
    { property: "details", label: "details", type: ContentType.String }
  ];

  const onContextMenu = (event: React.MouseEvent<HTMLLIElement>, {}, checkedRiskObjectRows: RiskObjectRow[]) => {
    const contextProps: RiskObjectContextMenuProps = { checkedRiskObjectRows, dispatchOperation, selectedServer, wellbore: selectedWellbore, servers };
    const position = getContextMenuPosition(event);
    dispatchOperation({ type: OperationType.DisplayContextMenu, payload: { component: <RiskObjectContextMenu {...contextProps} />, position } });
  };

  return Object.is(selectedWellbore?.risks, risks) && <ContentTable columns={columns} data={getTableData()} onContextMenu={onContextMenu} checkableRows />;
};

export default RisksListView;
