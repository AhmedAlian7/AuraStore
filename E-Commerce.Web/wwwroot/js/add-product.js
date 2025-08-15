document.addEventListener('DOMContentLoaded', function () {
    // Main image preview
    const mainImageInput = document.getElementById('mainImageInput');
    const mainImagePreview = document.getElementById('mainImagePreview');

    mainImageInput.addEventListener('change', function (e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                mainImagePreview.src = e.target.result;
                mainImagePreview.style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
    });

    // Additional images preview
    const additionalImagesInput = document.getElementById('additionalImagesInput');
    const additionalImagesPreview = document.getElementById('additionalImagesPreview');

    additionalImagesInput.addEventListener('change', function (e) {
        const files = Array.from(e.target.files);
        additionalImagesPreview.innerHTML = '';

        files.forEach((file, index) => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageItem = document.createElement('div');
                imageItem.className = 'additional-image-item';
                imageItem.innerHTML = `
                            <img src="${e.target.result}" class="additional-image-preview" alt="Additional image ${index + 1}">
                            <button type="button" class="remove-image-btn" onclick="removeAdditionalImage(${index})">
                                <i class="fas fa-times"></i>
                            </button>
                        `;
                additionalImagesPreview.appendChild(imageItem);
            };
            reader.readAsDataURL(file);
        });
    });

    // Drag and drop functionality
    const uploadAreas = document.querySelectorAll('.image-upload-area');
    uploadAreas.forEach(area => {
        area.addEventListener('dragover', function (e) {
            e.preventDefault();
            this.classList.add('dragover');
        });

        area.addEventListener('dragleave', function (e) {
            e.preventDefault();
            this.classList.remove('dragover');
        });

        area.addEventListener('drop', function (e) {
            e.preventDefault();
            this.classList.remove('dragover');

            const files = e.dataTransfer.files;
            if (this.onclick.toString().includes('mainImageInput') && files.length > 0) {
                mainImageInput.files = files;
                mainImageInput.dispatchEvent(new Event('change'));
            } else if (this.onclick.toString().includes('additionalImagesInput')) {
                additionalImagesInput.files = files;
                additionalImagesInput.dispatchEvent(new Event('change'));
            }
        });
    });
});

function removeAdditionalImage(index) {
    // This would need to be implemented with proper file handling
    const imageItems = document.querySelectorAll('.additional-image-item');
    if (imageItems[index]) {
        imageItems[index].remove();
    }
}