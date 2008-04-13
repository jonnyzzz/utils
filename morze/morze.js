JMZ = {

  table : {
   "à": ".-",
   "á": "-...",
   "â": ".--",
   "ã": "--.",
   "ä": "-..",
   "å": ".",
   "æ": "...-",
   "ç": "--..",
   "è": "..",
   "é": ".---",
   "ê": "-.-",
   "ë": ".-..",
   "ì": "--",
   "í": "-.", 
   "î": "---",
   "ï": ".--.",
   "ð": ".-.",
   "ñ": "...",
   "ò": "-",
   "ó": "..-",
   "ô": "..-.",
   "õ": "....",
   "ö": "-.-.",
   "÷": "---.",
   "ø": "----",
   "ù": "--.-",
   "û": "-.--",
   "ü": "-..-",
   "ý":	"..-.",
   "þ": "..--",
   "ÿ": ".-.-",  

   "1": ".----",
   "2": "..---",
   "3":	"...--",
   "4": "....-",
   "5": ".....",
   "6": "-....",
   "7":	"--...",
   "8": "---..",
   "9": "----.",
   "0": "-----",
   ".": "......",
   ",": ".-.-.-",
   "?": "..--..", 
   "!": "--..--",  
   "@": ".--.-."
  },

  tableBack : null,


  toMorze: function(c) {
   var out = "";
   c = c.toLowerCase();
   for(var i =0; i <c.length; i++) {
     var mapped = JMZ.table[c.charAt(i)];
     if (mapped != undefined) {
       out += mapped;
     } else {
       out += "<" + c.charAt(i) + ">";
     }    
     out += " ";
   }
   return out;
  },

  fromMorze : function(c) {
    var out = "";
    JMZ._initBack();
  
    c = c.replace("_", "-");
    
    var tokens = c.split(/\s+/);
    
    for(var i=0; i<tokens.length; i++) {
      if (tokens[i].length == 0)
       continue;

      var mapped = JMZ.tableBack[tokens[i]];
      if (mapped != null) {
       out += mapped;
      } else {
       out += "<" + tokens[i] + ">";
      }
    }   

    return out;

  },

  _initBack : function() {
     if (JMZ.tableBack != null) {
        return;
     }

     JMZ.tableBack = {};
     for(var p in JMZ.table) {
       JMZ.tableBack[JMZ.table[p]] = p;
     }

  }
};


