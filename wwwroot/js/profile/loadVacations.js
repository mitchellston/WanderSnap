const vacationsContainerElement = document
  .getElementById("vacations")
  .querySelector("div");

let index = 0;
vacationsContainerElement.childNodes.forEach((element) => {
  if ($(element).hasClass("ignore")) return;
  if (!$(element).is("div")) return;
  $.ajax({
    url: "?handler=Vacations",
    method: "GET",
    type: "GET",
    data: { which: index },
    success: ({ data, message, success }) => {
      console.log(data);
      $(element).html(`
        <a href="${window.location.href}/vacation/${
        data.id
      }" class="relative group hover:bg-slate-600 bg-slate-600 max-w-[100vw] w-96 h-60">
            <img class="z-[-1] w-full h-full object-center"
                src="${
                  data.images.length > 0
                    ? "/profile/vacations/" + data.images[0] ??
                      "https://via.placeholder.com/150"
                    : "https://via.placeholder.com/150"
                }"
                alt="${data.name} photo" />
            <div
                class="absolute bottom-0 p-0.5 text-ellipsis overflow-hidden h-1/3 bg-black bg-opacity-30 group-hover:bg-opacity-50 transition-all max-w-[100vw] text-white w-96">
                <p>Vacation: ${data.name}</p>
                <p class="">
                    Description: ${data.description}
                </p>
            </div>
        </a>`);
    },
  });
  index++;
});
