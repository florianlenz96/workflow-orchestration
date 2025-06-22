# Insurance Claim Processor - Demo UI

A modern, professional web interface for testing Azure Durable Functions HTTP endpoints that process insurance claims with image upload and description analysis using the async HTTP API pattern.

## 🎯 Conference Demo Features

- **Professional Design**: Modern, responsive UI perfect for conference presentations
- **Azure Durable Functions Integration**: Full support for async HTTP API pattern
- **Real-time Status Polling**: Automatic polling of orchestration status
- **Image Upload**: Drag & drop or click to upload images (converted to base64)
- **Live API Testing**: Connect to your Azure Function endpoint
- **Demo Mode**: Load sample data for offline demonstrations
- **Error Handling**: User-friendly error messages and validation
- **Responsive**: Works on desktop, tablet, and mobile devices

## 🚀 Quick Start

1. **Open the demo**: Double-click `index.html` or open it in your browser
2. **Update endpoint**: Edit `script.js` and replace the `API_ENDPOINT` URL with your Azure Function URL
3. **Test the interface**: Use the "Load Demo Data" button for offline testing

## 📁 File Structure

```
InsuranceUi/
├── index.html          # Main HTML page
├── styles.css          # Professional styling
├── script.js           # JavaScript functionality with Durable Functions support
└── README.md           # This file
```

## 🔧 Configuration

### Update Your Azure Function Endpoint

1. Open `script.js`
2. Find line 2: `const API_ENDPOINT = 'http://localhost:7102/api/ProcessStart';`
3. Replace with your actual Azure Function URL

Example:

```javascript
const API_ENDPOINT =
  "https://your-function-app.azurewebsites.net/api/ProcessStart";
```

### Azure Durable Functions Integration

This UI is specifically designed for Azure Durable Functions using the async HTTP API pattern:

#### Request Format

- **Method**: POST
- **Content-Type**: application/json
- **Payload**:

```json
{
  "Name": "unique-request-id",
  "Image": "base64-encoded-image-string",
  "Description": "Claim description text"
}
```

#### Expected Response (HTTP 202)

```json
{
  "Id": "orchestration-instance-id",
  "StatusQueryGetUri": "http://localhost:7102/runtime/webhooks/durabletask/instances/...",
  "SendEventPostUri": "http://localhost:7102/runtime/webhooks/durabletask/instances/...",
  "TerminatePostUri": "http://localhost:7102/runtime/webhooks/durabletask/instances/...",
  "PurgeHistoryDeleteUri": "http://localhost:7102/runtime/webhooks/durabletask/instances/..."
}
```

#### Status Polling

The UI automatically polls the `StatusQueryGetUri` every 2 seconds to check orchestration status:

```json
{
  "name": "Orchestrator",
  "instanceId": "instance-id",
  "runtimeStatus": "Completed",
  "input": { ... },
  "output": {
    "decision": false,
    "description": "Analysis result description",
    "manualProcessing": true
  },
  "createdTime": "2025-06-22T15:19:19Z",
  "lastUpdatedTime": "2025-06-22T15:19:41Z"
}
```

## 🎤 Conference Presentation Tips

### Demo Flow

1. **Introduction**: Show the professional UI design and explain the async pattern
2. **Upload Demo**: Use "Load Demo Data" button to show sample data
3. **Live Demo**: Upload a real image and show the orchestration flow
4. **Status Polling**: Demonstrate real-time status updates
5. **Final Result**: Show the completed analysis with decision

### Key Features to Highlight

- **Async Pattern**: Explain the 202 response and status polling
- **Real-time Updates**: Show status changes during processing
- **Image Processing**: Demonstrate base64 conversion and upload
- **Professional UI**: Emphasize the modern, production-ready design
- **Error Handling**: Demonstrate validation and orchestration error states

### Offline Demo Mode

If your Azure Function isn't deployed yet, use the "Load Demo Data" button (bottom-left corner) to show sample orchestration data and demonstrate the UI functionality.

## 🔄 Async HTTP API Pattern

### How It Works

1. **Initial Request**: POST to your function endpoint with image and description
2. **202 Response**: Function returns immediately with orchestration instance ID
3. **Status Polling**: UI polls the StatusQueryGetUri every 2 seconds
4. **Completion**: When runtimeStatus is "Completed", polling stops and results are displayed

### Supported Runtime Statuses

- **Pending**: Orchestration is starting
- **Running**: Orchestration is processing
- **Completed**: Orchestration finished successfully
- **Failed**: Orchestration encountered an error
- **Terminated**: Orchestration was manually terminated

## 🎨 Customization

### Colors and Branding

Edit `styles.css` to customize:

- Primary colors (currently purple gradient)
- Fonts (currently Inter)
- Logo and branding elements

### Demo Data

Modify the `loadDemoData()` function in `script.js` to show different sample data for your specific use case.

### Polling Interval

Change the polling frequency by modifying the interval in `startStatusPolling()` function (currently 2000ms).

## 🔒 Security Notes

- This is a demo interface - don't use in production without proper security measures
- Consider adding authentication for production use
- Validate file types and sizes on both client and server side
- Use HTTPS for all API communications
- Consider implementing proper CORS policies

## 📱 Browser Compatibility

- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## 🛠️ Development

To modify the interface:

1. Edit `index.html` for structure changes
2. Edit `styles.css` for visual changes
3. Edit `script.js` for functionality changes

The interface uses vanilla HTML, CSS, and JavaScript - no build process required!

## 📞 Support

For conference presentation support or customization requests, refer to the main project documentation.

---

**Built for Conference Presentation** | Insurance Claim Processor Demo with Azure Durable Functions
