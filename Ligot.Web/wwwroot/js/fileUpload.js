// File upload drag and drop helper
export function initializeFileDropZone(dropZoneId, inputFileId, dotNetHelper) {
    console.log('Initializing file drop zone with IDs:', dropZoneId, inputFileId);
    
    const dropZone = document.getElementById(dropZoneId);
    const inputFile = document.getElementById(inputFileId);
    
    if (!dropZone || !inputFile) {
        console.error('Drop zone or input file element not found', {
            dropZoneId: dropZoneId,
            dropZoneFound: !!dropZone,
            inputFileId: inputFileId,
            inputFileFound: !!inputFile
        });
        return false;
    }
    
    console.log('Drop zone and input file elements found successfully');

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    // Handle drop event
    dropZone.addEventListener('drop', function(e) {
        console.log('Drop event triggered');
        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length > 0) {
            console.log(`Processing ${files.length} dropped file(s)`);
            
            // Create a new DataTransfer object and add the files
            const dataTransfer = new DataTransfer();
            for (let i = 0; i < files.length; i++) {
                dataTransfer.items.add(files[i]);
                console.log(`Added file: ${files[i].name}`);
            }
            
            // Set the files to the input element
            inputFile.files = dataTransfer.files;
            
            // Trigger the change event on the InputFile component
            const event = new Event('change', { bubbles: true });
            inputFile.dispatchEvent(event);
            
            console.log(`${files.length} file(s) dropped and change event dispatched`);
        } else {
            console.log('No files in drop event');
        }
    });

    console.log('File drop zone initialized successfully');
    return true;
}

export function cleanupFileDropZone(dropZoneId) {
    const dropZone = document.getElementById(dropZoneId);
    if (dropZone) {
        // Remove all event listeners by cloning and replacing
        const newDropZone = dropZone.cloneNode(true);
        dropZone.parentNode.replaceChild(newDropZone, dropZone);
    }
}
