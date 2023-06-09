const confirmRemoveVacation = (id) => {
  const confirmDeleteModal = document.getElementById("confirmDeleteModal");
  confirmDeleteModal.showModal();
  $(confirmDeleteModal).data("id", id);
  $("#confirmedDeleteButton").on("click", () => {
    const id = $(confirmDeleteModal).data("id");
    console.log(id);
    $.ajax({
      url: "?handler=DeleteVacation",
      method: "POST",
      type: "POST",
      headers: {
        RequestVerificationToken: $(
          'input:hidden[name="__RequestVerificationToken"]'
        ).val(),
      },
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify({ which: id }),
      success: ({ data, message, success }) => {
        if (success) {
          $(`#vacation-${id}`).remove();
          confirmDeleteModal.close();
        } else {
          alert(message);
        }
      },
    });
  });
  $(confirmDeleteModal).on("close", () => {
    console.log("closed");
    $(confirmDeleteModal).data("id", null);
  });
};
