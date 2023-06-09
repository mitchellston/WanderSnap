const editData = {};
const editProfile = () => {
  $("#editProfile").html("");
  const saveButtonElement = document.createElement("div");
  saveButtonElement.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Save</button>`;
  saveButtonElement.addEventListener("click", saveProfile);
  const cancelButtonElement = document.createElement("div");
  cancelButtonElement.innerHTML = `<button class="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">Cancel</button>`;
  cancelButtonElement.addEventListener("click", cancelEdit);
  $("#editProfile").append(cancelButtonElement);
  $("#editProfile").append(saveButtonElement);
  const editableElements = document.querySelectorAll(".editable");
  for (let i = 0; i < editableElements.length; i++) {
    const column = $(editableElements[i]).data("column");
    const element = $(editableElements[i]).data("element");
    const type = $(editableElements[i]).data("type");
    const replaceElement = document.createElement(element);
    if (type !== null) replaceElement.type = type;
    replaceElement.value = editableElements[i].innerText;
    editData[column] = {
      replacedElement: replaceElement,
      formerElement: editableElements[i],
    };
    $(editableElements[i]).replaceWith(replaceElement);
  }
};
const dataToUpdate = {};

const saveProfile = () => {
  $("#editProfile").html("");
  for (let key in editData) {
    if (
      $(editData[key].replacedElement).val() !==
      $(editData[key].formerElement).text()
    ) {
      dataToUpdate[key] = $(editData[key].replacedElement).val();
    } else {
      dataToUpdate[key] = null;
    }
  }
  console.log(dataToUpdate);
  $.ajax({
    url: "?handler=EditProfile",
    type: "POST",
    headers: {
      RequestVerificationToken: $(
        'input:hidden[name="__RequestVerificationToken"]'
      ).val(),
    },
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(dataToUpdate),
    error: failedEdit,
    success: successEdit,
  });
};
const cancelEdit = () => {
  $("#editProfile").html("");
  const editButton = document.createElement("div");
  editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
  editButton.addEventListener("click", editProfile);
  $("#editProfile").append(editButton);
  for (let key in editData) {
    $(editData[key].replacedElement).replaceWith(editData[key].formerElement);
  }
};
$("#editProfile").children("button").on("click", editProfile);

const failedEdit = () => {
  const editButton = document.createElement("div");
  editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile <span class="text-red-700"> - Failed </span></button>`;
  editButton.addEventListener("click", editProfile);
  $("#editProfile").append(editButton);
  setTimeout(() => {
    editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
  }, 3000);
  for (let key in editData) {
    $(editData[key].replacedElement).replaceWith(editData[key].formerElement);
  }
};
const successEdit = ({ data, message, success }) => {
  if (success) {
    const editButton = document.createElement("div");
    editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
    editButton.addEventListener("click", editProfile);
    $("#editProfile").append(editButton);
    for (let key in editData) {
      const el = $(editData[key].formerElement);
      el.text($(editData[key].replacedElement).val());
      $(editData[key].replacedElement).replaceWith(el);
    }
  } else {
    window.alert(message);
    failedEdit();
  }
};
