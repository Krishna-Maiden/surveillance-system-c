# Maintenance & IoT Integration Wireframe Spec

## 1. Login/Authentication
- Logo and system name
- Username/email and password fields
- Login button
- Forgot password link
- (Optional) SSO/social login buttons

## 2. Maintenance Dashboard
- Top navigation bar (logo, user menu, notifications)
- Sidebar with main menu (Work Orders, Assets, PM, Inventory, IoT, Reports, Settings)
- Dashboard widgets:
  - Open work orders (by status)
  - Upcoming PM tasks
  - Asset health summary
  - Recent alerts/events
  - Quick links (create work order, add asset)

## 3. Work Orders
- Table/list of work orders (filter by status, type, asset, date)
- Create new work order button
- Bulk actions (assign, close, export)
- Click work order to view details

## 4. Create/Assign Work Order
- Form: asset, type, description, priority, due date
- Assign to technician/team
- Attach files/photos
- Save/cancel buttons

## 5. Work Order Details
- Work order info (status, asset, assigned, due date)
- Action/status update buttons (in progress, acknowledge, complete)
- Activity log (comments, status changes)
- Attachments
- Confirm handover button

## 6. Assets/Equipment
- Table/list of assets (filter by location, type, status)
- Add/edit/delete asset button
- Asset status indicators (active, under maintenance)
- Click asset to view details

## 7. Asset Details
- Asset info (location, specs, docs, warranty)
- Maintenance history (list of past work orders)
- PM checklist templates
- QR/barcode for quick access
- Edit asset button

## 8. Preventive Maintenance (PM)
- PM calendar/schedule view
- List of upcoming PM tasks
- Create/edit PM schedule button
- Assign checklist to asset
- PM status indicators

## 9. PM Checklist Execution
- Checklist items (custom per asset)
- Mark item as complete/failed
- Add notes/photos
- Save/submit checklist

## 10. Inventory/Parts
- Table/list of parts (filter by location, type, stock)
- Add/edit/delete part button
- Stock level indicators (low, reorder)
- Supplier info
- Reserve part for work order

## 11. IoT Monitoring
- Sensor data dashboard (real-time graphs, status indicators)
- Filter by asset, parameter, date
- Alerts for threshold breaches
- Device health/status
- Add/manage IoT device button

## 12. Real-time Alerts
- List of active/past alerts
- Alert details (type, asset, time, value)
- Acknowledge/resolve alert button
- Notification settings

## 13. Anomaly/Prediction
- List of detected anomalies/predictions
- Asset, parameter, predicted failure/time
- Root cause analysis tools
- Generate work order from anomaly

## 14. Reports & Analytics
- KPI dashboard (MTTR, MTBF, cost, OEE)
- Prebuilt and custom report templates
- Export/download (PDF, CSV)
- Schedule report button

## 15. Settings & Admin
- User list (add, edit, remove)
- Roles/permissions management
- System configuration (integrations, notifications)
- API/integration settings
- Compliance/audit log

## 16. User/Roles
- Role list and details
- Assign permissions to roles
- Assign users to roles

## 17. Integration/API
- API keys and documentation
- Integration status (ERP, EAM, SCADA)
- Data import/export tools

## 18. Compliance/Audit
- Audit trail of all actions
- Compliance report templates
- Data retention settings

---

**Navigation:**
- Sidebar for main navigation
- Top bar for user/account actions
- Breadcrumbs for sub-pages
- Consistent back/cancel/save buttons

**Mobile:**
- Responsive layout for all screens
- Collapsible sidebar
- Touch-friendly buttons and lists 