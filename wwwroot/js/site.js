
"use strict";
(function (_w, $jq) {

    _w.post = function (u, d, s, f) {

        var $apiProxy = {
            "async": true,
            "crossDomain": true,
            "url": "",
            "method": "POST",
            "headers": {
                "Content-Type": "application/json",
                "cache-control": "no-cache"
            },
            "processData": false,
            "data": ""
        };
        $apiProxy.data = d;
        $apiProxy.url = u;
        $apiProxy.beforeSend = function (xhr) {
        
        }
        $.post($apiProxy).done(s).fail(function (x, e) {
            
        });
    };
}
)(window, jQuery);

var ovr =
{
    d: {},
    on: function () {
        ovr.d.rw = new Array();
        $("#btnsovr").click(ovr.new);
        $("#txshemp").keyup(ovr.empsearch).blur(function () {
            window.setTimeout(function () {
                $("#txshemp").val('').parent().find(".search-pn").remove();
            }, 500);
        });
        post("/ovr/getlist", null, function (r) {
            if (typeof r == "string") {
                $("#ovrlist").html(r);
                $("#bckit_tab_ovr").bcKitTab();
            }
        });
        post("/ovr/category/get", null, function (r) {
            var _sel = $("#ovrcat");
            $.each(r.data, function (k, v) {
                $('<option/>').attr("value", "").html("Select").appendTo(_sel);
                var _og = $('<optgroup/>').attr("label", v.header);
                _og.appendTo(_sel);
                $.each(v.values, function (i, j) {
                    $('<option/>').attr("value", i).html(j).appendTo(_og);
                });
            });
            $(_sel).select2();
        });
        $("#btnovr").click(function () {
            ovr.loadCategories();
            ui_mpanel.show("bckit-modal-new", 'New Ovr', {});
        });
        $("#btnClr").click(function () {
            alert("Confirm: Would you like to reset all fields ?");
            clearForm();
        });
       
        function clearForm() {
           
            $('input[type="text"], textarea').val('');
            $('select').prop('selectedIndex', 0);

            $('input[type="checkbox"], input[type="radio"]').prop('checked', false);
            $("#dvsremp").empty();
            $('#date').val('');
            $('#time').val('');
    
            $('#fileList').html('');
            $('#ifile').val(''); 
            if ($('#description').trumbowyg) {
                $('#description').trumbowyg('empty'); 
            }      
            $('.form-control-req').removeClass('form-control-req');
            if ($('#ImmediateCorrection').trumbowyg) {
                $('#ImmediateCorrection').trumbowyg('empty');
            }
            $('.form-control-req').removeClass('form-control-req');

            
           
        }

        $("#filetrigger").click(function () { $("#ifile").click(); });
        $("#ifile").change(
            function () {
                $("#fileList").html('');
                var f = $(this)[0];

                var files = f.files;
                var formData = new FormData();
                var fileSize = 0;
                var list = $("<ul/>");
                for (var i = 0; i != files.length; i++) {
                    fileSize += files[i].size;
                    $(list).append($("<li/>").html(files[i].name));
                    
                }

                fileSize = (fileSize / 1024)
                if (fileSize > 5120) {
                    alert("File size is exceeded")
                    return;
                }
                $("#fileList").append(list);

            }

        );

        $('textarea.form-control').trumbowyg({
            btns: [
                ['strong', 'em', 'del'],
                ['justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull'],
                ['unorderedList', 'orderedList'],
                ['fullscreen']
            ]
        }
        );
    },
    new: function () {
        var data = {};
        var fl = false;
        $("#ovrn").find(".form-control").each(
            function (i, v) {
                if ($(this).data("req") == "1") {
                    if ($(this).val() === "") {
                        $(this).addClass("form-control-req");
                        fl = true;

                    }
                    else {
                        $(this).removeClass("form-control-req");
                    }
                }

                var name = $(this).attr('id');
                var value = $(this).val();
                data[name] = value;

            }
        );
        if (fl) {
            alert("Please fill the required details");
            return;
        }
        if (ovr.d.rw.length == 0) {
            alert("Please add atleast 1 witness");
            return;
        }

        data["description"] = $('#description').trumbowyg('html');
        post("/ovr/request/save", JSON.stringify(
            {
                "Details": data, "Witness": ovr.d.rw
            }
        ),
            function (r) {
                if (r.status) {
                    var data = r.data;
                    ovr.uploadFile("ifile", r.tKey, function () { alert("success") });
                    $("#ovrn").find(".form-control").val('');
                    ui_mpanel.hide();

                    Swal.fire({
                        title: "OVR Request",
                        text: "OVR Request has been created",
                        icon: "success"
                    });
                    window.setTimeout(function () {
                        location.reload();
                    }, 3000);
                }
                else {
                    Swal.fire({
                        title: "Error",
                        text: r.message,
                        icon: "error"
                    });

                }
            });
    },
    loadCategories: function () {
        post("/ovr/categories", JSON.stringify(null),
            function (r) {
                if (r && r.length > 0) {

                    const $categorySelect = $('#userclassification');
                    $categorySelect.empty();

                    $categorySelect.append('<option value="">Select Category</option>');

                    $.each(r, function (index, category) {
                        $categorySelect.append($('<option></option>').val(category.CatID).text(category.Category));
                    });
                }
                else {
                    alert("No categories found.");
                }
            });
            
    },
   

    empsearch: function () {
        var value = $(this).val();
        if (value.length < 3) { return; }
        $(this).parent().find(".search-pn").remove();
        post("/ovr/emp/search", JSON.stringify({ "text": value }),
            function (r) {
                if (r.status) {
                    if (r.data == null) { return; }
                    var list = r.data.rows;
                    var e = $("#txshemp").parent(),
                        _e = $("<div/>"),
                        _u = $("<ul/>");
                    _u.appendTo(_e);
                    _e.addClass("search-pn").appendTo(e);
                    $(list).each(function (i, v) {
                        let _h = "<label>" + v[2] + " (" + v[1] + ")</label>";
                        _h += "<label>" + v[5] + ", " + v[4] + "</label>";
                        $("<li/>").html(_h).appendTo(_u).click(
                            function () {
                                let eid = v[0];
                                let en = v[2];
                                var lb = $("<label/>").addClass("itemsel").html("x | " + en);
                                ovr.d.rw.push(eid);
                                $("#dvsremp").append(lb);
                                (function (aid, ar) {
                                    $(lb).click(function () {
                                        const index = ar.indexOf(aid);
                                        if (index > -1) {
                                            ar.splice(index, 1);
                                        }
                                        $(this).remove();
                                    });
                                })(eid, ovr.d.rw);
                            }
                        );
                    });
                }
                else {
                    alert(r.message);
                }
            });
    },

 
    wcupdate: function () {
        post("/ovr/action/witnesscomnt", JSON.stringify(
            {
                "comment": $("#tawc").val(), "ovrid": $("#_ovr").val()
            }
        ),
            function (r) {
                if (r.status) {
                    alert('Saved');
                    location.reload();
                }
                else {
                    alert(r.message);
                }
            });
    },
    acrequ: function () {
        var data = {};
        data["ovrid"] = $("#_ovr").val();
        data["auth"] = ovr.d.rw[0];
        data["comnt"] = $("#tanetactn").val();
        var sel = $("#rctionsel").val();

        post("/ovr/actionr/request", JSON.stringify(
            {
                "Details": data, "Witness": sel
            }
        ),
            function (r) {
                if (r.status) {
                    alert('Saved');
                    location.reload();
                }
                else {
                    alert(r.message);
                }
            });
    },
    acreqact: function (t) {
        if (!confirm("Are you sure want to update ?.")) {
            return;
        }
        var data = {};
        data["id"] = $("#_ovr").val();
        data["type"] = t;
        if (t == "T") {
            data["value"] = t == "T"
            $("#selicdtype").val()
        }
        else {
            var v = $("#ovrcat").val();
            data["value"] = v.join();
        }
        post("/ovr/actionp/updtc", JSON.stringify(data),
            function (r) {
                if (r.status) {
                    alert('Saved');
                    location.reload();
                }
                else {
                    alert(r.message);
                }
            });
    },
    actreqcomnt: function () {
        if (!confirm("Are you sure want to update ?.")) {
            return;
        }
        var data = {};
        data["id"] = $("#selactrtype").val();
        data["type"] = $("#selactrtype :selected").text();
        data["ovrid"] = $("#_ovr").val();
        data["comment"] = $("#talactrtype").val();
        post("/ovr/actionr/comnt", JSON.stringify(data),
            function (r) {
                if (r.status) {
                    ovr.uploadFile("ifile", r.tKey, function () { alert("File uploaded") });
                    location.reload();
                }
                else {
                    alert(r.message);
                }
            });
    },
    uploadFile: function (src, key, sfn) {
        var input = document.getElementById(src);
        if (input.files.length == 0) {
            return;
        }
        var files = input.files;
        var formData = new FormData();
        var fileSize = 0;
        for (var i = 0; i != files.length; i++) {
            fileSize += files[i].size;
            formData.append("files", files[i]);
        }
        fileSize = (fileSize / 1024)
        if (fileSize > 5120) {
            alert("File size is exceeded")
            return;
        }
        $.ajax(
            {
                url: "/ovr/savfiles",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                "headers": {
                    "Authorization": key
                },
                success: function (data) {
                    sfn();
                }
            }
        );
    },

}
$('#userclassification').change(function () {
    const catId = $(this).val();
    if (catId) {
        post("/ovr/Subcategories", JSON.stringify({ "CatID": catId }), function (r) {
            const $subcategorySelect = $('#Subcategory');
            $subcategorySelect.empty();
            $subcategorySelect.append('<option value="">Select Subcategory</option>');
            if (r && r.length > 0) {
                $.each(r, function (index, subcategory) {
                    $subcategorySelect.append($('<option></option>').val(subcategory.CatID).text(subcategory.Subcategory));
                });
            } else {
                alert("No subcategories found.");
            }

        });
    }
});

$(document).ready(function () {
    const $tableBody = $('tbody');
    const $filters = $('input, select');

    function filterTable() {
        const $rows = $tableBody.find('tr');

        $rows.each(function () {
            let $row = $(this);
            let visible = true;

            $filters.each(function (index) {
                const cellValue = $row.children('td').eq(index + 1).text().trim();
                let filterValue = $(this).val().trim();

                if ($(this).attr('type') === 'date') {
                    const filterDate = new Date(filterValue);
                    const filterDateString = `${(filterDate.getMonth() + 1).toString().padStart(2, '0')}/${filterDate.getDate().toString().padStart(2, '0')}/${filterDate.getFullYear()}`;
                    if (filterValue && cellValue !== filterDateString) {
                        visible = false;
                    }
                } else {
                    if (filterValue && !cellValue.toLowerCase().includes(filterValue.toLowerCase())) {
                        visible = false;
                    }
                }
            });

            $row.toggle(visible);
        });
    }

    $filters.on('input', filterTable);
    $('#reset-btn').on('click', function () {
        $filters.val('');
        filterTable();
    });
    $('#export-btn').on('click', function () {
        const exl = XLSX.utils.table_to_book(document.querySelector('table'), { sheet: "Sheet 1" });
        XLSX.writeFile(exl, 'TableData.xlsx');
    });
    $('#refresh-btn').on('click', function () {
        location.reload();
        window.scrollTo(0, 0);

            });
});
$(document).ready(
    function () {
        ovr.on();
    }
);

