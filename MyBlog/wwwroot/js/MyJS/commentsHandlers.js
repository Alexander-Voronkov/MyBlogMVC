
async function mainCommentHandler(event) {

    event.preventDefault();

    let form = document.forms.mainCommentForm;
    let message = form.elements.Message.value;
    let postId = form.elements.postId.value;

    const responce = await fetch("/Comments/CreateComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            message: message,
            postId: postId
        })
    });

    if (responce) {

        recountComments(1);

        const resultPartialView = await responce.text();
        document.getElementById("mainCommentId").innerHTML += resultPartialView;
    }
    else {
        console.log(responce.statusText);
    }
}


async function replyCommentHandler(event, commentId) {

    event.preventDefault();

    console.log(commentId);
    let formName = "replyCommentForm".concat(commentId);

    let form = document.forms[formName]; //.replyCommentForm;
    let message = form.elements.Message.value;
    let postId = form.elements.postId.value;
    let parentCommentId = form.elements.parentCommentId.value;
    let currentNested = form.elements.currentNested.value;
    let isReply = true;

    const response = await fetch("/Comments/CreateComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            message: message,
            postId: postId,
            parentCommentId: parentCommentId,
            currentNested: currentNested,
            isReply: isReply
        })
    });

    // console.log(event.target.parentNode.parentNode.parentNode);

    if (response) {

        recountComments(1);

        let parentBlock = event.target.parentNode.parentNode.parentNode;
        const writeParentCommentAndChildrenPartial = await response.text();
        parentBlock.innerHTML += writeParentCommentAndChildrenPartial;

        // document.getElementById("collapseReplyForm".concat(commentId)).classList.add("collapsing");
    }
    else {
        console.log(response.statusText);
    }
}

async function deleteCommentHandler(event, commentId) {

    event.preventDefault();

    const response = await fetch("/Comments/DeleteComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify(
            commentId
        )
    });

    if (response.ok) {

        //заклинанием ниже преобразуем текст ответа в минусовое число 
        recountComments(-Number(await response.text()));

        let parentBlock = event.target.parentNode.parentNode.parentNode;
        //parentBlock.remove();
        parentBlock.classList.add("removedTransform");

        parentBlock.addEventListener("transitionend", () => {
            parentBlock.className = ""; //remove();
            parentBlock.innerHTML = "<h3 class='bg-danger rounded rounded-3 ps-3'>Comment Was Deleted</h3>";
        });
    }
    else {
        console.log(response);
    }
}




function recountComments(count) {

    let commentsCountEl = document.getElementById("commentsCountId");
    let commentsCount = commentsCountEl.innerText;
    commentsCount = +commentsCount + count;
    commentsCountEl.innerText = commentsCount;
}