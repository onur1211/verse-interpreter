grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( expr) * EOF;

term : ID ':' INTTYPE ';' 
     | ID '=' INT ';' 
     ;

expr : term expr 
     | term 
     | ID '+' ID ';'
     | ID
     ;
