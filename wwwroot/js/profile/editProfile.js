const editData = {};
const editProfile = () => {
  $("#editProfile").html("");
  // create save and cancel button
  const saveButtonElement = document.createElement("div");
  saveButtonElement.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Save</button>`;
  saveButtonElement.addEventListener("click", saveProfile);
  const cancelButtonElement = document.createElement("div");
  cancelButtonElement.innerHTML = `<button class="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">Cancel</button>`;
  cancelButtonElement.addEventListener("click", cancelEdit);
  $("#editProfile").append(cancelButtonElement);
  $("#editProfile").append(saveButtonElement);
  // replace text elements with input elements
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
  // add profile picture edit input
  $("#profilePicture").append(
    `<input type="file" id="updateProfilePicture" />`
  );
};
const dataToUpdate = new FormData();

const saveProfile = () => {
  $("#editProfile").html("");
  // set data to update
  for (let key in editData) {
    if (
      $(editData[key].replacedElement).val() !==
      $(editData[key].formerElement).text()
    ) {
      dataToUpdate.append(key, $(editData[key].replacedElement).val());
    }
  }
  // add profile picture to data if it is uploaded
  const updateProfilePictureInput = document.getElementById(
    "updateProfilePicture"
  );
  if (
    updateProfilePictureInput &&
    updateProfilePictureInput.files &&
    updateProfilePictureInput.files[0]
  ) {
    dataToUpdate.append("profilePicture", updateProfilePictureInput.files[0]);
  }
  // ajax call to edit profile
  $.ajax({
    url: "?handler=EditProfile",
    type: "POST",
    headers: {
      RequestVerificationToken: $(
        'input:hidden[name="__RequestVerificationToken"]'
      ).val(),
    },
    data: dataToUpdate,
    error: failedEdit,
    success: successEdit,
    processData: false,
    contentType: false, // Using FormData, no need to process data.
  });
};
const cancelEdit = () => {
  // create edit button
  $("#editProfile").html("");
  const editButton = document.createElement("div");
  editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
  editButton.addEventListener("click", editProfile);
  $("#editProfile").append(editButton);
  // replace input elements with the former elements
  for (let key in editData) {
    $(editData[key].replacedElement).replaceWith(editData[key].formerElement);
  }
  // remove profile picture edit input
  $("#updateProfilePicture").remove();
};

const failedEdit = () => {
  // create edit button
  const editButton = document.createElement("div");
  editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile <span class="text-red-700"> - Failed </span></button>`;
  editButton.addEventListener("click", editProfile);
  $("#editProfile").append(editButton);
  // remove failed message after 3 seconds
  setTimeout(() => {
    editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
  }, 3000);
  // replace input elements with the former elements
  for (let key in editData) {
    $(editData[key].replacedElement).replaceWith(editData[key].formerElement);
  }
  // remove profile picture edit input
  $("#updateProfilePicture").remove();
};
const successEdit = ({ data, message, success }) => {
  // if succesfully edited
  if (success) {
    // create edit button
    const editButton = document.createElement("div");
    editButton.innerHTML = `<button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Edit profile</button>`;
    editButton.addEventListener("click", editProfile);
    $("#editProfile").append(editButton);
    // replace input elements with text elements
    for (let key in editData) {
      const el = $(editData[key].formerElement);
      el.text($(editData[key].replacedElement).val());
      $(editData[key].replacedElement).replaceWith(el);
    }
    // update profile picture
    if (data.find((x) => x._column === "profile_picture")) {
      $("#profilePicture").html(
        "<img src='/uploads/profile/profilePics/" +
          data.find((x) => x._column === "profile_picture")._value +
          "' class='rounded-full object-cover w-32 h-32' />"
      );
    } else {
      $("#profilePicture").html(
        "<img src='/media/profile/NOPROFILE.jpg' class='rounded-full object-cover w-32 h-32' />"
      );
    }
  } else {
    // if failed to edit show error message and call failedEdit
    window.alert(message);
    failedEdit();
  }
};
// Event listeners
$("#editProfile").children("button").on("click", editProfile);
