function deletePlace(id) {
	if (confirm(`Delete ${id}?`)) {
		window.open(`/Admin/Delete?id=${id}`, '_self');
	}
}

function deleteComment(id) {
	if (confirm(`Delete ${id}?`)) {
		window.open(`/Admin/DeleteComment?id=${id}`, '_self');
	}
}