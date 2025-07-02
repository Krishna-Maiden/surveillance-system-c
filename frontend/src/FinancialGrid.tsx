import React, { useRef, useState, MouseEvent } from "react";
import { Modal, Button } from "./Modal";
import { BarChart, Bar, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer } from "recharts";
import { ModuleRegistry, AllCommunityModule } from 'ag-grid-community';
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";

ModuleRegistry.registerModules([AllCommunityModule]);

const rowData = [
  { jan: 38031, feb: 49473, mar: 68014, apr: 10370, may: 37787, jun: 71057, jul: 71433 },
  { jan: 17697, feb: 2324, mar: 93400, apr: 65628, may: 40729, jun: 16473, jul: 4900 },
  { jan: 54892, feb: 57821, mar: 30621, apr: 19621, may: 88908, jun: 33077, jul: 29398 },
  { jan: 81081, feb: 16825, mar: 89225, apr: 89225, may: 92547, jun: 46989, jul: 54510 },
  { jan: 73947, feb: 41410, mar: 30148, apr: 30148, may: 90558, jun: 9844, jul: 55200 },
  { jan: 20479, feb: 2324, mar: 93400, apr: 65628, may: 40729, jun: 16473, jul: 1149 },
  { jan: 44446, feb: 34410, mar: 30148, apr: 30148, may: 90558, jun: 72878, jul: 72878 },
];

const columnDefs = [
  { headerName: "Jan", field: "jan" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "Feb", field: "feb" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "Mar", field: "mar" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "Apr", field: "apr" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "May", field: "may" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "Jun", field: "jun" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
  { headerName: "Jul", field: "jul" as keyof typeof rowData[0], filter: "agNumberColumnFilter" },
];

const defaultColDef = {
  resizable: true,
  sortable: true,
  filter: true,
  flex: 1,
  valueFormatter: (params: { value: number }) => params.value ? `$${params.value.toLocaleString()}` : "",
};

const customMenuStyle: React.CSSProperties = {
  position: 'fixed',
  background: '#fff',
  border: '1px solid #ccc',
  boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
  zIndex: 2000,
  minWidth: 180,
  padding: 0
};

export default function FinancialGrid() {
  const gridRef = useRef<any>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [chartData, setChartData] = useState<any[]>([]);
  const [menu, setMenu] = useState<{x: number, y: number, show: boolean}>({x: 0, y: 0, show: false});

  // Custom right-click handler for AG Grid cells
  const onGridContextMenu = (params: any) => {
    params.event.preventDefault();
    setMenu({ x: params.event.clientX, y: params.event.clientY, show: true });
  };

  const handleMenuAction = (action: string) => {
    setMenu(m => ({ ...m, show: false }));
    const api = gridRef.current?.api;
    if (!api) return;
    if (action === 'copy') api.copySelectedRangeToClipboard(false);
    if (action === 'copyWithHeaders') api.copySelectedRangeToClipboard(true);
    if (action === 'export') api.exportDataAsCsv();
    if (action === 'chart') {
      const cellRanges = api.getCellRanges();
      if (cellRanges && cellRanges.length > 0) {
        const range = cellRanges[0];
        const rowStart = Math.min(range.startRow.rowIndex, range.endRow.rowIndex);
        const rowEnd = Math.max(range.startRow.rowIndex, range.endRow.rowIndex);
        const colIds = range.columns.map((col: any) => col.colId);
        const data: any[] = [];
        for (let i = rowStart; i <= rowEnd; i++) {
          const rowNode = api.getDisplayedRowAtIndex(i);
          const rowObj: any = { name: `Row ${i + 1}` };
          colIds.forEach((colId: string) => {
            rowObj[colId] = rowNode.data[colId];
          });
          data.push(rowObj);
        }
        setChartData(data);
        setModalOpen(true);
      }
    }
  };

  const onCellMouseDown = (params: any) => {
    if (params.event.button === 0) {
      setMenu(m => m.show ? { ...m, show: false } : m);
    }
  };

  return (
    <div style={{ width: "100%", height: "100vh", position: 'relative' }}>
      <div
        className="ag-theme-alpine"
        style={{ height: 600, width: "100%" }}
      >
        <AgGridReact
          ref={gridRef}
          rowData={rowData}
          columnDefs={columnDefs}
          defaultColDef={defaultColDef}
          enableRangeSelection={true}
          onCellContextMenu={onGridContextMenu}
          onCellMouseDown={onCellMouseDown}
        />
      </div>
      {menu.show && (
        <ul style={{ ...customMenuStyle, left: menu.x, top: menu.y, listStyle: 'none', margin: 0 }}>
          <li style={{ padding: '8px 16px', cursor: 'pointer' }} onClick={() => handleMenuAction('copy')}>Copy</li>
          <li style={{ padding: '8px 16px', cursor: 'pointer' }} onClick={() => handleMenuAction('copyWithHeaders')}>Copy with Headers</li>
          <li style={{ padding: '8px 16px', cursor: 'pointer' }} onClick={() => handleMenuAction('export')}>Export</li>
          <li style={{ padding: '8px 16px', cursor: 'pointer' }} onClick={() => handleMenuAction('chart')}>Chart Range</li>
        </ul>
      )}
      <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
        <h2>Bar Chart</h2>
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={chartData}>
            <XAxis dataKey="name" />
            <YAxis />
            <Tooltip />
            <Legend />
            {columnDefs.map(col => (
              <Bar key={col.field as string} dataKey={col.field as string} fill="#8884d8" />
            ))}
          </BarChart>
        </ResponsiveContainer>
        <Button onClick={() => setModalOpen(false)}>Close</Button>
      </Modal>
    </div>
  );
} 