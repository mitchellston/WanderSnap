const photoContainerElement = document.getElementById("photos");

let index = 0;
let done = 0;

photoContainerElement.childNodes.forEach((element) => {
  if ($(element).hasClass("ignore")) return Done();

  if (!$(element).is("div")) return Done();

  $.ajax({
    url: "?handler=Vacations",
    method: "GET",
    type: "GET",
    data: { which: index },
    success: async ({ data, message, success }) => {
      Done();
      if (!success || data == null) return console.error(message);
      $(element).html(`
        <div class="relative  w-96 group hover:bg-slate-600 bg-slate-600 max-w-[100vw]">
            <img class="z-[-1] w-full h-full object-center"
                src="${"/profile/vacations/" + data.path}"
                alt="${data.description} photo" />
            <div
                class="absolute bottom-0 p-0.5 text-ellipsis overflow-hidden h-20 max-h-[30%] bg-black bg-opacity-30 group-hover:bg-opacity-50 transition-all max-w-[100vw] text-white w-96">
                <p>
                    Description: ${data.description}
                </p>
            </div>
        </div>`);
    },
  });
  index++;
});
function Done() {
  done++;
  if (done == photoContainerElement.childNodes.length - 1) {
    $(photoContainerElement).removeClass("gap-1 flex flex-wrap justify-center");
    $(photoContainerElement).addClass(
      "gap-1 columns-1 min-[2180px]:columns-5 min-[1795px]:columns-4 min-[1415px]:columns-3 min-[1030px]:columns-2"
    );
  }
}
