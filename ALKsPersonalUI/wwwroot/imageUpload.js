// Extracts browser-provided webkitRelativePath values from a file input element
// because Blazor's InputFile strips relative paths before creating IBrowserFile instances.
export function getRelativePaths(inputElement) {
    if (!inputElement || !inputElement.files) {
        return [];
    }

    const files = inputElement.files;
    const paths = new Array(files.length);

    for (let index = 0; index < files.length; index++) {
        const rawPath = files[index].webkitRelativePath;
        paths[index] = rawPath ? rawPath.replace(/\\/g, "/") : null;
    }

    return paths;
}

