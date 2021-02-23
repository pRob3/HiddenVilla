window.ShowToastr = (type, message) => {
    if (type === "success") {
        // Display a success toast, with a title
        toastr.success(message, 'Operation Successful', { timeOut: 10000 });
    }
    if (type === "error") {
        // Display an error toast, with a title
        toastr.error(message, 'Operation Failure', { timeOut: 10000 });
    }
};

window.ShowSweetalert = (type, message) => {
    if (type === "success") {
        Swal.fire(
            'Success!',
            message,
            'success'
        )
    }
    if (type === "error") {
        Swal.fire(
            'Error!',
            message,
            'error'
        )
    }
};

function ShowDeleteConfirmationModal() {
    $('#deleteConfirmationModal').modal('show');
};

function HideDeleteConfirmationModal() {
    $('#deleteConfirmationModal').modal('hide');
};