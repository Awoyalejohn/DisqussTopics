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

            // Initiate ajax request with fetch API to display the data for the partial view
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

    // Bootstrap toast
    if (document.getElementById('liveToast') != null) {
        const toastLiveExample = document.getElementById('liveToast')

        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLiveExample)

        toastBootstrap.show()
    }

    // Modal pop up for post share
    if (document.querySelectorAll(".post-share-modal") != null) {
        // Get all the post share modals
        let postShareModals = document.querySelectorAll(".post-share-modal")

        for (let i = 0; i < postShareModals.length; i++) {
            postShareModals[i].addEventListener("show.bs.modal", () => {
                console.log(postShareModals[i]);
                console.log(postShareModals[i].getAttribute("data-post-share-modal-id"));

                // Get the  Id of the post
                let postId = postShareModals[i].getAttribute("data-post-share-modal-id");
                let url = `/Post/Share/${postId}`;
                console.log(url);

                // Initiate ajax request with fetch API to display the data for the partial view
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
                        console.log(postShareModals[i])
                        let sharePostPartialContainer = document.querySelector(`#modal-share-post-${postId}`);
                        console.log(sharePostPartialContainer);
                        sharePostPartialContainer.innerHTML = html;
                        console.log(sharePostPartialContainer);

                        if (document.querySelectorAll(".copy-link-button") != null) {
                            // Get copy link buttons
                            let copyLinkButtons = document.querySelectorAll(".copy-link-button");
                            console.log(copyLinkButtons);

                            for (let i = 0; i < copyLinkButtons.length; i++) {
                                copyLinkButtons[i].addEventListener("click", () => {
                                    // Get the text input
                                    let copyText = document.querySelector(`.url-${postId}`);

                                    // Select the text field
                                    copyText.select();
                                    copyText.setSelectionRange(0, 99999); // For mobile devices

                                    // Copy the text inside the text field
                                    navigator.clipboard.writeText(copyText.value);

                                    console.log(copyText);
                                    console.log(copyLinkButtons[i]);
                                });
                            }
                        }
                    })
                    .catch((error) => {
                        console.log(error);
                    });

            });
        }
    }

    if (document.querySelectorAll(".copy-link-button") != null) {
        // Get copy link buttons
        let copyLinkButtons = document.querySelectorAll(".copy-link-button");
        console.log(copyLinkButtons);

        for (let i = 0; i < copyLinkButtons.length; i++) {
            copyLinkButtons[i].addEventListener("click", () => {
                // Get the text input
                //let copyText = document.querySelector(`.url-${postId}`);
                //console.log(copyText);
                console.log(copyLinkButtons[i]);
            });
        }

    }

});
