# bEmu

bEmu é uma plataforma de emulação desenvolvida do zero em .NET 5, inspirada nos projetos RetroArch/Libretro e MAME. A ideia principal do projeto é ser uma plataforma extensível de emulação, capaz de oferecer reaproveitamento de componentes em comum entre os sistemas como processadores ou chipsets, por exemplo. 

O bEmu possui uma estrutura voltada para a implementação de novos sistemas sem muito esforço arquitetural, provendo uma base de implementações úteis e mantendo uma base de código dos sistemas emulados baseada apenas em funcionalidades nativas do .NET Standard. Dessa forma, é fácil migrar a estrutura do projeto, que hoje está baseada em MonoGame, para outras tecnologias, sem comprometer as implementações particulares dos sistemas emulados.

Todo o código encontrado no bEmu é escrito do zero, sem uso de bibliotecas externas para realizar tarefas relacionadas à emulação, como por exemplo importar uma biblioteca de terceiros que implementa um interpretador. O motivo dessa decisão é o foco no aprendizado a fundo de como funcionam os sistemas, que juntamente com o processo de pesquisa e desenvolvimento são objetivos fundamentais do projeto, até mais do que o resultado final do sistema sendo emulado. 

É um trabalho em constante evolução conduzido por mim nas minhas horas vagas, primeiramente por hobby e como uma forma de aprendizado sobre os sistemas e videogames antigos. Com certeza há muitos bugs e imprecisões nas plataformas emuladas, até mesmo funcionalidades não implementadas ou não implementadas da melhor forma possível. Entretanto, existe sempre o objetivo de fazer a qualidade das emulações ser cada vez melhor e mais precisa.

**Sistemas atualmente suportados**

- GameBoy
- GameBoy Color
- Arcades baseados em Space Invaders (Intel 8080)
- Chip8
- SuperChip8

**Roadmap**

- Melhorar a qualidade de emulação do GameBoy/GameBoy Color
- Desenvolver o suporte a áudio nativo nos sistemas atuais
- Desenvolver a implementação do processador Z80
- Desenvolver a emulação do arcade original de Pac-Man
- Desenvolver a emulação do Nintendo Entertainment System (NES)
- Desenvolver a emulação do SG-1000, primeiro console da Sega
- Desenvolver a emulação de outros arcades baseados em Intel 8080
- Melhorar continuamente a performance e a qualidade da emulação
- Disponibilizar mais melhorias para a interface gráfica do bEmu