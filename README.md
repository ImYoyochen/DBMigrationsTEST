# 人事管理系統與檔案上傳功能 - Web API 專案

這是一個基於 `.NET 8` 的 Web API 專案，實現了人事管理系統與檔案上傳功能。

## 專案主要功能
- **員工管理**
  - 完整的 CRUD 操作
  - 資料庫遷移與預設資料
- **檔案管理**
  - 安全的檔案上傳
  - 檔案驗證與儲存功能

## 使用的技術
- .NET 8
- Entity Framework Core
- MySQL 資料庫
- Swagger API 文件
- Docker 容器化支援

## 安裝需求
- .NET 8 SDK
- MySQL Server
- Docker（選配）

## 使用說明
1. **設定資料庫連線字串**：在 `appsettings.json` 中更新連線設定。
2. **執行資料庫遷移指令**：  
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. **設定檔案上傳參數**：配置允許的檔案副檔名與大小限制。

## API 端點

### 員工管理
- **取得所有員工**  
  `GET /api/employees`
- **取得特定員工**  
  `GET /api/employees/{id}`
- **新增員工**  
  `POST /api/employees`
- **更新員工**  
  `PUT /api/employees/{id}`
- **刪除員工**  
  `DELETE /api/employees/{id}`

### 檔案管理
- **上傳檔案**  
  `POST /api/files/upload`
- **下載檔案**  
  `GET /api/files/{fileName}`
- **刪除檔案**  
  `DELETE /api/files/{fileName}`

## 安全性考量
- **檔案上傳大小限制**：避免過大檔案影響系統運作。
- **檔案類型驗證**：只允許特定類型檔案進行上傳。
- **檔案內容驗證**：確保檔案內容不包含惡意程式碼。
- **檔案名稱安全處理**：避免特殊字元造成安全問題。
- **防止目錄遍歷攻擊**：嚴格限制檔案存取範圍。

## 擴展性
- 使用介面設計模式，支持儲存服務的彈性替換（如從本地儲存改為雲端儲存服務）。

## 相關文件
更多體驗內容可參考：[Corsur 體驗過程](https://docs.google.com/document/d/1_mENRUi27O8H1M7e0-BPz2sUJqrwINZYnxZInzPksbw/edit?usp=sharing)

