var grandTotal;
$(document).ready(function () {
    $('#quantityInput').on('paste copy', function (event) {
        event.preventDefault();
    });
    $('#quantityInput').on('input', function () {
        var quantity = $(this).val();
        if (!isNaN(quantity) && quantity >= 0) {
            var costPerItem = 7.25;
            var totalAmount = quantity * costPerItem;
            $('#amountInput').val(totalAmount);
        } else {
            $('#amountInput').val('Invalid input');
        }
    });
});
$('#proceedBtn').on('click', function () {
    var quantity = $('#quantityInput').val();
    if (!isNaN(quantity) && quantity >= 0) {
        var costPerItem = 7.25;
        var gstPercentage = 18;
        var gstPerItem = (costPerItem * gstPercentage) / 100;
        var totalCost = quantity * costPerItem;
        var totalGST = quantity * gstPerItem;
         grandTotal = totalCost + totalGST;
        $("#cost1").text(totalCost.toFixed(2));
        $("#gst1").text(totalGST.toFixed(2));
        $("#totalt1").text(grandTotal.toFixed(2));
        $('#grandtotal1').text(grandTotal.toFixed(2));
        $('.amountGrid').css('display', 'block');
    } else {
        $('.amountGrid').css('display', 'none');
    }
});
function ProceedToPayment() {
    
    $.ajax({
        url: '/MySigns/Signature',
        type: 'POST',
        data: {
            AmountRupees: grandTotal
        },
        success: function (result) {
            
            if (result == 0) {
                $('#paymentGateway').modal('show');
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}










function onlyNumbers(event) {
    var charCode = event.charCode || event.keyCode;
    if (charCode < 48 || charCode > 57 || charCode == 46 || charCode == 45) {
        event.preventDefault();
    }
}

var monthYearInput = document.getElementById('monthyear');
monthYearInput.addEventListener('input', function () {
    
    var sanitizedInput = this.value.replace(/[^0-9]/g, '');
    if (sanitizedInput.length > 2) {
        var month = sanitizedInput.substring(0, 2);
        var year = sanitizedInput.substring(2, 4);
        if (parseInt(month) >= 1 && parseInt(month) <= 12 && parseInt(year) >= 0 && parseInt(year) <= 99) {
            this.value = month + '/' + year;
        } else {
            this.value = '';
        }
    } else {
        this.value = sanitizedInput;
    }
    this.value = this.value.toUpperCase();
});
$('#cardnumber').on('input', function () {
    var cardNumber = $(this).val().replace(/-/g, '');
    if (cardNumber.length > 0) {
        cardNumber = cardNumber.match(new RegExp('.{1,4}', 'g')).join('-');
    }
    $(this).val(cardNumber);
});
var UPIID = $("#UPIID").val();
var typingTimer;
var doneTypingInterval = 3000;

$("#UPIID").on('input', function () {
    clearTimeout(typingTimer);
    typingTimer = setTimeout(validateUpiId, doneTypingInterval);
});

function validateUpiId() {
    debugger
    var UPIID = $("#UPIID").val();
    if (isValidUpiId(UPIID)) {
        console.log("UPI ID is valid");
    } else {
        console.log("Invalid UPI ID");
    }
}

function isValidUpiId(UPIID) {
    var upiPattern = /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9.-]+$/;
    return upiPattern.test(UPIID);
}

function isValidNetBankingID(netBankingID) {
    var netBankingIDPattern = /^[a-zA-Z0-9]{6,12}$/;
    return netBankingIDPattern.test(netBankingID);
}




