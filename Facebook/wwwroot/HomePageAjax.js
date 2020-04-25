function showcomment(id)
{
    $.ajax
        (
            {
                method: "Post",
                Url: "Getcomment",
                data: {
                    PostId: id
                },
                success: function (responsecomment)
                {
                    document.getElementById("getcom").innerHTML = response;
                }
        }
    )
}
function showlike(id)
{
    $.ajax
        (
            {
                method: "Post",
                Url: "Getlike",
                data: { PostId: id }
                success: function (responselike)
                {
                    document.getElementById("like").innerHTML = responselike;
                }
            }
        )
}