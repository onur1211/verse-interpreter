grammar Verse;
options {tokenVocab=VerseLexer;
         output=template;
         ASTLabelType = StringTemplate;
}

verse_text: ( expr) * EOF;

term : ID ':' INTTYPE ';' 
     | ID '=' INT ';' 
     ;

expr : term expr 
     | term 
     | ID '+' ID ';'
     | ID
     ;