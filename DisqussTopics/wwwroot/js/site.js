
addEventListener("DOMContentLoaded", () => {

    // Get all the comment edit modals
    let modals = document.querySelectorAll(".comment-edit-modal")

    for (let i = 0; i < modals.length; i++) {
        modals[i].addEventListener("show.bs.modal", function () {
            console.log(modals[i]);
            console.log(modals[i].getAttribute("data-comment-edit-modal-id"));

            // Get the  Id of the comment
            let commentId = modals[i].getAttribute("data-comment-edit-modal-id");
            let url = `/Post/Edit/Comment/${commentId}`;
            console.log(url);

            // Initiate ajax request with fetch API display the data for the partial view
            fetch(url)
                .then((response) => {
                    if (response.ok) {
                        return response.text();
                    } else {
                        throw new Error('An error occurred while loading the partial view.');
                    }
                })
                .then((html) => {
                    console.log(html);
                    console.log(modals[i]);
                    let mymodal = document.querySelector(`[data-comment-edit-modal-id='${commentId}']`);
                    console.log(mymodal);
                    mymodal.innerHTML = html;
                    console.log(mymodal);
                })
                .catch((error) => {
                    console.log(error);
                });
        });
    }
});