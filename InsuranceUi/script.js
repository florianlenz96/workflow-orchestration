// Configuration - Update this URL to your Azure Function endpoint
const API_ENDPOINT =
  "https://dwx-insurance-workflow.azurewebsites.net/api/ProcessStart?code=VTBSyd-BiDMHgV8JlSmH76AjUJ39O_u4I32kXwpil4_mAzFu-RY4Gg==";

// DOM elements
const claimForm = document.getElementById("claimForm");
const descriptionInput = document.getElementById("description");
const imageInput = document.getElementById("image");
const uploadArea = document.getElementById("uploadArea");
const imagePreviewContainer = document.getElementById("imagePreviewContainer");
const imagePreview = document.getElementById("imagePreview");
const removeImageBtn = document.getElementById("removeImage");
const submitBtn = document.getElementById("submitBtn");
const clearBtn = document.getElementById("clearBtn");
const responseContainer = document.getElementById("responseContainer");
const statusIndicator = document.getElementById("statusIndicator");
const requestInfo = document.getElementById("requestInfo");
const responseData = document.getElementById("responseData");
const decisionSection = document.getElementById("decisionSection");
const decisionIndicator = document.getElementById("decisionIndicator");
const decisionDescription = document.getElementById("decisionDescription");
const toggleAnalysisBtn = document.getElementById("toggleAnalysisBtn");
const analysisContent = document.getElementById("analysisContent");

// State
let selectedFile = null;
let currentInstanceId = null;
let statusPollingInterval = null;

// Initialize the application
document.addEventListener("DOMContentLoaded", function () {
  initializeEventListeners();
  setupDragAndDrop();
});

function initializeEventListeners() {
  // Form submission
  claimForm.addEventListener("submit", handleFormSubmit);

  // Image selection
  imageInput.addEventListener("change", handleImageSelect);

  // Remove image
  removeImageBtn.addEventListener("click", removeSelectedImage);

  // Clear form
  clearBtn.addEventListener("click", clearForm);

  // Upload area click
  uploadArea.addEventListener("click", () => imageInput.click());

  // Toggle analysis content
  toggleAnalysisBtn.addEventListener("click", toggleAnalysisContent);
}

function setupDragAndDrop() {
  // Prevent default drag behaviors
  ["dragenter", "dragover", "dragleave", "drop"].forEach((eventName) => {
    uploadArea.addEventListener(eventName, preventDefaults, false);
    document.body.addEventListener(eventName, preventDefaults, false);
  });

  // Highlight drop area when item is dragged over it
  ["dragenter", "dragover"].forEach((eventName) => {
    uploadArea.addEventListener(eventName, highlight, false);
  });

  ["dragleave", "drop"].forEach((eventName) => {
    uploadArea.addEventListener(eventName, unhighlight, false);
  });

  // Handle dropped files
  uploadArea.addEventListener("drop", handleDrop, false);
}

function preventDefaults(e) {
  e.preventDefault();
  e.stopPropagation();
}

function highlight(e) {
  uploadArea.classList.add("dragover");
}

function unhighlight(e) {
  uploadArea.classList.remove("dragover");
}

function handleDrop(e) {
  const dt = e.dataTransfer;
  const files = dt.files;

  if (files.length > 0) {
    const file = files[0];
    if (file.type.startsWith("image/")) {
      handleFileSelect(file);
    } else {
      showError("Please select an image file.");
    }
  }
}

function handleImageSelect(e) {
  const file = e.target.files[0];
  if (file) {
    handleFileSelect(file);
  }
}

function handleFileSelect(file) {
  // Validate file type
  if (!file.type.startsWith("image/")) {
    showError("Please select a valid image file.");
    return;
  }

  // Validate file size (max 10MB)
  const maxSize = 10 * 1024 * 1024; // 10MB
  if (file.size > maxSize) {
    showError("File size must be less than 10MB.");
    return;
  }

  selectedFile = file;
  displayImagePreview(file);
}

function displayImagePreview(file) {
  const reader = new FileReader();
  reader.onload = function (e) {
    imagePreview.src = e.target.result;
    imagePreviewContainer.style.display = "block";
    uploadArea.style.display = "none";
  };
  reader.readAsDataURL(file);
}

function removeSelectedImage() {
  selectedFile = null;
  imageInput.value = "";
  imagePreviewContainer.style.display = "none";
  uploadArea.style.display = "block";
  imagePreview.src = "";
}

function clearForm() {
  claimForm.reset();
  removeSelectedImage();
  hideResponse();
  stopStatusPolling();
  hideDecisionSection();
}

function toggleAnalysisContent() {
  const isVisible = analysisContent.style.display !== "none";

  if (isVisible) {
    analysisContent.style.display = "none";
    toggleAnalysisBtn.classList.remove("expanded");
  } else {
    analysisContent.style.display = "block";
    toggleAnalysisBtn.classList.add("expanded");
  }
}

function hideDecisionSection() {
  decisionSection.style.display = "none";
}

function showDecisionSection(decision, description, requiresManualProcessing) {
  // Update decision indicator
  const decisionText = decisionIndicator.querySelector(".decision-text");
  const decisionIcon = decisionIndicator.querySelector("i");

  let decisionLabel = "";
  let iconClass = "";

  if (decision === true) {
    decisionIndicator.className = "decision-indicator approved";
    decisionLabel = "Genehmigt";
    iconClass = "fas fa-check-circle";
  } else if (decision === false) {
    decisionIndicator.className = "decision-indicator rejected";
    decisionLabel = "Abgelehnt";
    iconClass = "fas fa-times-circle";
  } else {
    decisionIndicator.className = "decision-indicator manual";
    decisionLabel = "Manuelle Prüfung erforderlich";
    iconClass = "fas fa-exclamation-triangle";
  }

  // Add processing type information
  const processingType = requiresManualProcessing
    ? "Manuell bearbeitet"
    : "KI-automatisiert";
  decisionText.textContent = `${decisionLabel} (${processingType})`;
  decisionIcon.className = iconClass;

  // Update description
  decisionDescription.textContent =
    description || "Keine Beschreibung verfügbar.";

  // Show the decision section
  decisionSection.style.display = "block";
}

async function handleFormSubmit(e) {
  e.preventDefault();

  const description = descriptionInput.value.trim();

  if (!description) {
    showError("Please enter a description.");
    return;
  }

  if (!selectedFile) {
    showError("Please select an image.");
    return;
  }

  // Clear previous results
  hideDecisionSection();
  analysisContent.style.display = "none";
  toggleAnalysisBtn.classList.remove("expanded");
  responseData.textContent = "";

  // Show loading state
  setLoadingState(true);
  showResponse();

  try {
    // Convert image to base64
    const base64Image = await fileToBase64(selectedFile);

    // Prepare request payload
    const payload = {
      Name: generateUniqueId(),
      Image: base64Image,
      Description: description,
    };

    // Log request details
    logRequestDetails(description, selectedFile, payload.Name);

    // Start the orchestration
    const response = await fetch(API_ENDPOINT, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (response.status === 202) {
      const result = await response.json();
      currentInstanceId = result.Id;

      // Display the initial response
      displayOrchestrationStarted(result);

      // Start polling for status updates
      startStatusPolling(result.StatusQueryGetUri);
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error:", error);
    displayError(error.message);
    setLoadingState(false);
  }
}

function fileToBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      // Remove the data URL prefix (e.g., "data:image/jpeg;base64,")
      const base64 = reader.result.split(",")[1];
      resolve(base64);
    };
    reader.onerror = (error) => reject(error);
  });
}

function generateUniqueId() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c == "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

function startStatusPolling(statusQueryUri) {
  // Clear any existing polling
  stopStatusPolling();

  // Start polling every 2 seconds
  statusPollingInterval = setInterval(async () => {
    try {
      const response = await fetch(statusQueryUri);

      if (response.ok) {
        const status = await response.json();
        updateStatusDisplay(status);

        // Stop polling if the orchestration is completed
        if (
          status.runtimeStatus === "Completed" ||
          status.runtimeStatus === "Failed" ||
          status.runtimeStatus === "Terminated"
        ) {
          stopStatusPolling();
          setLoadingState(false);

          if (status.runtimeStatus === "Completed") {
            displayFinalResult(status);
          } else {
            displayError(`Orchestration ${status.runtimeStatus.toLowerCase()}`);
          }
        }
      } else {
        console.error("Status query failed:", response.status);
      }
    } catch (error) {
      console.error("Error polling status:", error);
    }
  }, 2000);
}

function stopStatusPolling() {
  if (statusPollingInterval) {
    clearInterval(statusPollingInterval);
    statusPollingInterval = null;
  }
}

function updateStatusDisplay(status) {
  const statusMessages = {
    Running: "Processing claim...",
    Completed: "Processing completed!",
    Failed: "Processing failed",
    Terminated: "Processing terminated",
    Pending: "Starting processing...",
  };

  const message =
    statusMessages[status.runtimeStatus] || `Status: ${status.runtimeStatus}`;
  updateStatus(status.runtimeStatus.toLowerCase(), message);

  // Update response data with current status
  const statusInfo = {
    instanceId: status.instanceId,
    runtimeStatus: status.runtimeStatus,
    createdTime: status.createdTime,
    lastUpdatedTime: status.lastUpdatedTime,
    input: status.input,
    output: status.output,
    customStatus: status.customStatus,
  };

  responseData.textContent = JSON.stringify(statusInfo, null, 2);
}

function displayOrchestrationStarted(result) {
  updateStatus("processing", "Orchestration started - polling for updates...");

  const startInfo = {
    message: "Orchestration started successfully",
    instanceId: result.Id,
    statusQueryUri: result.StatusQueryGetUri,
    availableActions: {
      terminate: result.TerminatePostUri,
      suspend: result.SuspendPostUri,
      resume: result.ResumePostUri,
      purge: result.PurgeHistoryDeleteUri,
    },
  };

  responseData.textContent = JSON.stringify(startInfo, null, 2);
}

function displayFinalResult(status) {
  updateStatus("success", "Processing completed successfully!");

  // Show decision section
  showDecisionSection(
    status.output?.decision,
    status.output?.description,
    status.output?.manualProcessing
  );

  // Only show analysis result in the response data
  const analysisResult = {
    analysis: {
      decision: status.output?.decision,
      description: status.output?.description,
      requiresManualProcessing: status.output?.manualProcessing,
    },
  };

  responseData.textContent = JSON.stringify(analysisResult, null, 2);
}

function calculateProcessingTime(createdTime, lastUpdatedTime) {
  const created = new Date(createdTime);
  const updated = new Date(lastUpdatedTime);
  const diffMs = updated - created;
  const diffSeconds = Math.round(diffMs / 1000);
  return `${diffSeconds} seconds`;
}

function setLoadingState(isLoading) {
  const submitText = submitBtn.querySelector("span");
  const submitIcon = submitBtn.querySelector("i");

  if (isLoading) {
    submitBtn.disabled = true;
    submitIcon.className = "fas fa-spinner fa-spin";
    submitText.textContent = "Processing...";
  } else {
    submitBtn.disabled = false;
    submitIcon.className = "fas fa-paper-plane";
    submitText.textContent = "Process Claim";
  }
}

function showResponse() {
  responseContainer.style.display = "block";
  responseContainer.scrollIntoView({ behavior: "smooth" });
}

function hideResponse() {
  responseContainer.style.display = "none";
}

function updateStatus(type, message) {
  statusIndicator.className = `status-indicator ${type}`;
  statusIndicator.querySelector(".status-text").textContent = message;
}

function logRequestDetails(description, file, requestId) {
  const requestDetails = {
    timestamp: new Date().toISOString(),
    requestId: requestId,
    description: description,
    fileName: file.name,
    fileSize: formatFileSize(file.size),
    fileType: file.type,
  };

  requestInfo.textContent = JSON.stringify(requestDetails, null, 2);
}

function displayError(message) {
  updateStatus("error", "Processing failed");
  responseData.textContent = `Error: ${message}\n\nPlease check your Azure Function endpoint and try again.`;
}

function showError(message) {
  // Create a temporary error notification
  const notification = document.createElement("div");
  notification.className = "error-notification";
  notification.innerHTML = `
        <i class="fas fa-exclamation-triangle"></i>
        <span>${message}</span>
        <button onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;

  // Add styles
  notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #fed7d7;
        color: #e53e3e;
        padding: 15px 20px;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        display: flex;
        align-items: center;
        gap: 10px;
        z-index: 1000;
        max-width: 400px;
        animation: slideIn 0.3s ease;
    `;

  // Add animation styles
  const style = document.createElement("style");
  style.textContent = `
        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
    `;
  document.head.appendChild(style);

  document.body.appendChild(notification);

  // Auto-remove after 5 seconds
  setTimeout(() => {
    if (notification.parentElement) {
      notification.remove();
    }
  }, 5000);
}

function formatFileSize(bytes) {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
}
