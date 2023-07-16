jQuery(function ($) {
    $(document).on('click', '#addToCart', function () {

        var id = $(this).data('id');
        $.ajax({
            method: "GET",
            url: "/basket/add",
            data: {
                id : id
            },
            success: function (res) {
                alert(res)
            },
            error: function (err) {
                alert(err.responseText)
            }
        })
    })


    $(document).on('click', '#increase', function (e) {

        e.preventDefault();

        var increase = $(this);
        var id = $(this).data('id');
        $.ajax({
            method: "GET",
            url: "/basket/increase",
            data: {
                id: id
            },
            success: function (res) {
                var count = $($(increase).parent().siblings()[3]).html();
                count++;
                $($(increase).parent().siblings()[3]).html(count);
            },
            error: function (err) {
                alert(err.responseText)
            }
        })
    })


    $(document).on('click', '#decrease', function (e) {

        e.preventDefault();

        var decrease = $(this);
        var id = $(this).data('id');
        $.ajax({
            method: "GET",
            url: "/basket/decrease",
            data: {
                id: id
            },
            success: function (res) {
                var count = $($(decrease).parent().siblings()[3]).html();
                count--;
                $($(decrease).parent().siblings()[3]).html(count);
            },
            error: function (err) {
                alert(err.responseText)
            }
        })
    })


    $(document).on('click', '#delete', function (e) {

        e.preventDefault();

        var id = $(this).data('id');
        $.ajax({
            method: "GET",
            url: "/basket/delete",
            data: {
                id: id
            },
            success: function (res) {
                $(`.table tr[id = ${id}]`).remove();   
            },
            error: function (err) {
                alert(err.responseText)
            }
        })
    })
})